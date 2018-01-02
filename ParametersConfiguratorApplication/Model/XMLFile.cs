using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;


namespace ParametersConfiguratorApplication.Model
{
    public static class XMLFile
    {
        private static XNamespace XMLSchemaNS = XNamespace.Get(Properties.Settings.Default["XMLSchemaNS"].ToString());
        private static string XMLSchemaPath = Properties.Settings.Default["XMLSchemaPath"].ToString();
        private static SignsOfSet globalSignsOfSet = new SignsOfSet
        {
            _beginningSign = "[",
            _separatingSign = ";",
            _endingSign = "]"
        };
        public static XDocument ValidateXMLFile(string filePath)
        {
            XmlTextReader reader = new XmlTextReader(XMLSchemaPath);
            XmlSchema schema = XmlSchema.Read(reader, null);

            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(schema);
            XDocument xml = XDocument.Load(filePath, LoadOptions.PreserveWhitespace);

            bool errors = false;
            string errorStack = "Plik nie jest zgodny ze schematem XML.\nSchemat jest dostępny w podkatalogu \"schema\" w katalogu aplikacji pod nazwą \"parametersConfigurator.xsd\"\n\n";

            xml.Validate(schemas, (o, e) =>
            {
                errorStack += e.Message + "\n\n";
                errors = true;
            }, true);
            if (errors)
            {
                errorStack += "\nLista błędnych węzłów oraz atrybutów:\nSkrót {pc} odpowiada {http://www.czadzik.pl/parametersConfigurator}\n";
                errorStack = DumpInvalidNodes(xml.Root, errorStack);
                throw new Exception(errorStack);
            }
            return xml;
        }
        static string DumpInvalidNodes(XElement el, string errorsStack)
        {
            if (el.GetSchemaInfo().Validity != XmlSchemaValidity.Valid)
                errorsStack += "\n" + "Invalid Element " + el.AncestorsAndSelf().InDocumentOrder().Aggregate("", (s, i) => s + "/" + i.Name.ToString()).Replace("{http://www.czadzik.pl/parametersConfigurator}", "{pc}");
            foreach (XAttribute att in el.Attributes())
                if (att.GetSchemaInfo() != null && att.GetSchemaInfo().Validity != XmlSchemaValidity.Valid)
                    errorsStack += "\n" + "Invalid Attribute " + att.Parent.AncestorsAndSelf().InDocumentOrder().Aggregate("", (s, i) => s + "/" + i.Name.ToString().Replace("{http://www.czadzik.pl/parametersConfigurator}", "{pc}")) + "/@" + att.Name.ToString().Replace("{http://www.czadzik.pl/parametersConfigurator}", "{pc}");
            foreach (XElement child in el.Elements())
                errorsStack = DumpInvalidNodes(child, errorsStack);
            return errorsStack;
        }

        public static ParametersConfigurator Load(XDocument xml)
        {
            try
            {
                XElement parametersConfigurationElement = xml.Root;
                string parametersConfiguratorTitle = GetElementValue<string>(parametersConfigurationElement, XMLSchemaNS + "ParametersConfiguratorTitle", default(string));
                string parametersConfiguratorIntroduction = GetElementValue<string>(parametersConfigurationElement, XMLSchemaNS + "ParametersConfiguratorIntroduction", default(string));

                globalSignsOfSet = GetSignsOfSet(parametersConfigurationElement, XMLSchemaNS + "GlobalSignsOfSet");

                List<Section> sections = GetSections(parametersConfigurationElement);
                ConditionalSection correctnessConditions = GetLogicalObjects(parametersConfigurationElement, XMLSchemaNS + "CorrectnessConditions");

                string parametersConfiguratorSummary = GetElementValue<string>(parametersConfigurationElement, XMLSchemaNS + "ParametersConfiguratorSummary", default(string));

                bool additionalParameters = GetAttribute<bool>(parametersConfigurationElement, "additionalParameters", false);

                List<Parameter> allParameters = ListParameters(sections);

                SetParametersInVisibilityConditions(sections, allParameters);
                CheckConditionsClauseTypeInVisibilityConditions(sections);
                ComputeVisibilityOfParameters(sections, true);
                AddRegExConditions(correctnessConditions, allParameters);
                SetParametersInCorrectnessConditions(correctnessConditions, allParameters);
                CheckConditionsClausesTypesInCorrectness(correctnessConditions);
                return new ParametersConfigurator(parametersConfiguratorTitle, parametersConfiguratorIntroduction, sections, correctnessConditions,
                                                    parametersConfiguratorSummary, additionalParameters);
            }
            catch (Exception exc)
            {
                Console.WriteLine("Błąd podczas parsowania pliku XML.\n", exc.Message);
                return null;
            }
        }
        private static List<Section> GetSections(XElement sourceElement)
        {
            List<Section> sections = new List<Section>();
            XElement sectionsElement = sourceElement.Element(XMLSchemaNS + "Sections");

            if (sectionsElement != null)
            {
                foreach (XElement sectionElement in sectionsElement.Elements(XMLSchemaNS + "Section"))
                {
                    sections.Add(GetSection(sectionElement));
                }
            }

            return sections;
        }
        private static Section GetSection(XElement sourceElement)
        {
            ObservableCollection<Parameter> parameters = new ObservableCollection<Parameter>();
            XElement parametersElement = sourceElement.Element(XMLSchemaNS + "Parameters");

            foreach (XElement parametrElement in parametersElement.Elements())
            {
                parameters.Add(GetParameter(parametrElement));
            }

            LogicalObject visibilityCondition;
            XElement visibilityConditionElement = sourceElement.Element(XMLSchemaNS + "VisibilityCondition");
            if (visibilityConditionElement != null)
            {
                visibilityCondition = GetCondition(visibilityConditionElement);
            }
            else
            {
                visibilityCondition = new TrueLogicalObject();
            }

            return new Section(parameters, visibilityCondition);
        }
        private static Parameter GetParameter(XElement sourceElement)
        {
            Parameter parameter;

            string name = GetElementValue<string>(sourceElement, XMLSchemaNS + "Name", default(string));
            string label = GetElementValue<string>(sourceElement, XMLSchemaNS + "Label", default(string));
            string description = GetElementValue<string>(sourceElement, XMLSchemaNS + "Description", default(string));
            SignsOfSet signsOfSet = GetSignsOfSet(sourceElement, XMLSchemaNS + "SignsOfSet");
            List<Section> sections = GetSections(sourceElement);

            bool isOptional = GetAttribute<bool>(sourceElement, "optional", false);
            int minNumberOfValues = GetAttribute<int>(sourceElement, "minNumberOfValues", 1);
            int maxNumberOfValues = GetAttribute<int>(sourceElement, "maxNumberOfValues", 1);

            switch (sourceElement.Name.LocalName.ToString())
            {
                case "BooleanParameter":
                    {
                        parameter = new BooleanParameter(name, label, description, signsOfSet, sections, isOptional, minNumberOfValues, maxNumberOfValues);
                        break;
                    }
                case "IntegerParameter":
                    {
                        int minValue = GetElementValue<int>(sourceElement, XMLSchemaNS + "MinValue", Int32.MinValue);
                        int maxValue = GetElementValue<int>(sourceElement, XMLSchemaNS + "MaxValue", Int32.MaxValue);
                        parameter = new IntegerParameter(name, label, description, signsOfSet, sections, isOptional, minNumberOfValues, maxNumberOfValues, minValue, maxValue);
                        break;
                    }
                case "FloatParameter":
                    {
                        double minValue = GetElementValue<double>(sourceElement, XMLSchemaNS + "MinValue", Double.MinValue);
                        double maxValue = GetElementValue<double>(sourceElement, XMLSchemaNS + "MaxValue", Double.MaxValue);
                        parameter = new FloatParameter(name, label, description, signsOfSet, sections, isOptional, minNumberOfValues, maxNumberOfValues, minValue, maxValue);
                        break;
                    }
                case "EnumerationParameter":
                    {
                        List<string> items = GetItems(sourceElement, XMLSchemaNS + "EnumerationList");
                        parameter = new EnumerationParameter(name, label, description, signsOfSet, sections, isOptional, minNumberOfValues, maxNumberOfValues, items);
                        break;
                    }
                case "TextParameter":
                    {
                        string regularExpression = GetElementValue<string>(sourceElement, XMLSchemaNS + "RegularExpression", default(string));
                        parameter = new TextParameter(name, label, description, signsOfSet, sections, isOptional, minNumberOfValues, maxNumberOfValues, regularExpression);
                        break;
                    }
                case "DateTimeParameter":
                    {
                        DateTime minValue = GetElementValue<DateTime>(sourceElement, XMLSchemaNS + "MinValue", DateTime.MinValue);
                        DateTime maxValue = GetElementValue<DateTime>(sourceElement, XMLSchemaNS + "MaxValue", DateTime.MaxValue);
                        parameter = new DateTimeParameter(name, label, description, signsOfSet, sections, isOptional, minNumberOfValues, maxNumberOfValues, minValue, maxValue);
                        break;
                    }
                case "DateParameter":
                    {
                        DateTime minValue = GetElementValue<DateTime>(sourceElement, XMLSchemaNS + "MinValue", DateTime.MinValue);
                        DateTime maxValue = GetElementValue<DateTime>(sourceElement, XMLSchemaNS + "MaxValue", DateTime.MaxValue);
                        parameter = new DateParameter(name, label, description, signsOfSet, sections, isOptional, minNumberOfValues, maxNumberOfValues, minValue, maxValue);
                        break;
                    }
                default:
                    throw new Exception("Invalid type of parameter.");

            }
            return parameter;
        }
        private static List<string> GetItems(XElement enumerationElement, XName setName)
        {
            List<string> items = new List<string>();
            XElement enumerationList = enumerationElement.Element(setName);

            if (enumerationList != null)
            {
                foreach (XElement item in enumerationList.Elements(XMLSchemaNS + "Item"))
                {
                    items.Add(item.Value);
                }
            }
            return items;
        }
        private static ConditionalSection GetLogicalObjects(XElement sourceElement, XName conditionSectionName)
        {
            LogicalConnector logicalConnector = GetAttribute<LogicalConnector>(sourceElement, "logicalConnector", LogicalConnector.AND);
            XElement conditionalSectionElement = sourceElement.Element(conditionSectionName);
            if (conditionalSectionElement != null)
            {
                return GetLogicalObjects(conditionalSectionElement);
            }
            return new ConditionalSection(new List<LogicalObject>(), logicalConnector);
        }
        private static ConditionalSection GetLogicalObjects(XElement conditionalSectionElement)
        {
            LogicalConnector logicalConnector = GetAttribute<LogicalConnector>(conditionalSectionElement, "logicalConnector", LogicalConnector.AND);

            List<LogicalObject> logicalObjects = new List<LogicalObject>();

            foreach (XElement condition in conditionalSectionElement.Elements(XMLSchemaNS + "Condition"))
            {
                logicalObjects.Add(GetCondition(condition));
            }

            foreach (XElement conditionalSection in conditionalSectionElement.Elements(XMLSchemaNS + "ConditionalSection"))
            {
                logicalObjects.Add(GetLogicalObjects(conditionalSection));
            }

            return new ConditionalSection(logicalObjects, logicalConnector);
        }
        private static LogicalConnector GetLogicalConnector(string logicalConnectorString)
        {
            switch (logicalConnectorString)
            {
                case "AND":
                    return LogicalConnector.AND;
                case "OR":
                    return LogicalConnector.OR;
                default:
                    throw new Exception("Łącznik logiczny " + logicalConnectorString + " nie jest obsługiwany.");
            }
        }
        private static Condition GetCondition(XElement sourceElement)
        {
            string firstParameterName = GetElementValue<string>(sourceElement, XMLSchemaNS + "FirstParameterName", default(string));
            TypeOfClause clause = GetClause(sourceElement);

            string secondParameterValue = GetElementValue<string>(sourceElement, XMLSchemaNS + "Value", default(string));

            if (secondParameterValue != default(string))
            {
                return new Condition(firstParameterName, clause, null, secondParameterValue, null, TypeOfSecondParameter.Value);
            }
            string secondParameterName = GetElementValue<string>(sourceElement, XMLSchemaNS + "SecondParameterName", default(string));

            if (secondParameterName != default(string))
            {
                return new Condition(firstParameterName, clause, secondParameterName, null, null, TypeOfSecondParameter.ParameterName);
            }

            XElement set = sourceElement.Element(XMLSchemaNS + "Set");
            if (set != null)
            {
                return new Condition(firstParameterName, clause, null, null, GetItems(sourceElement, XMLSchemaNS + "Set"), TypeOfSecondParameter.Set);
            }

            throw new Exception("Błąd przy tworzeniu klasy Condition podczas parsowania pliku XML.");
        }
        private static TypeOfClause GetClause(XElement sourceElement)
        {
            string clauseString = GetElementValue<string>(sourceElement, XMLSchemaNS + "Clause", default(string));

            switch (clauseString)
            {
                case "EqualTo":
                    return TypeOfClause.EqualTo;
                case "eq":
                    return TypeOfClause.EqualTo;
                case "==":
                    return TypeOfClause.EqualTo;
                case "NotEqualTo":
                    return TypeOfClause.NotEqualTo;
                case "ne":
                    return TypeOfClause.NotEqualTo;
                case "!=":
                    return TypeOfClause.NotEqualTo;
                case "GreaterThan":
                    return TypeOfClause.GreaterThan;
                case "gt":
                    return TypeOfClause.GreaterThan;
                case ">":
                    return TypeOfClause.GreaterThan;
                case "GreaterOrEqual":
                    return TypeOfClause.GreaterOrEqual;
                case "ge":
                    return TypeOfClause.GreaterOrEqual;
                case ">=":
                    return TypeOfClause.GreaterOrEqual;
                case "LessThan":
                    return TypeOfClause.LessThan;
                case "lt":
                    return TypeOfClause.LessThan;
                case "<":
                    return TypeOfClause.LessThan;
                case "&lt;":
                    return TypeOfClause.LessThan;
                case "LessOrEqual":
                    return TypeOfClause.LessOrEqual;
                case "le":
                    return TypeOfClause.LessOrEqual;
                case "<=":
                    return TypeOfClause.LessOrEqual;
                case "&lt;=":
                    return TypeOfClause.LessOrEqual;
                case "IsSubset":
                    return TypeOfClause.IsSubset;
                case "EmptyIntersection":
                    return TypeOfClause.EmptyIntersection;
                case "NotEmptyIntersection":
                    return TypeOfClause.NotEmptyIntersection;
                default:
                    throw new Exception("Błędna klauzula warunkowa.");
            }
        }
        private static SignsOfSet GetSignsOfSet(XElement sourceElement, XName elementName)
        {
            XElement signsOfSetElement = sourceElement.Element(elementName);
            if (signsOfSetElement != null)
            {
                string beginningSign = GetElementValue<string>(signsOfSetElement, XMLSchemaNS + "BeginningSign", "[");
                string separatingSign = GetElementValue<string>(signsOfSetElement, XMLSchemaNS + "SeparatingSign", ";");
                string endingSign = GetElementValue<string>(signsOfSetElement, XMLSchemaNS + "EndingSign", "]");

                return new SignsOfSet
                {
                    _beginningSign = beginningSign,
                    _separatingSign = separatingSign,
                    _endingSign = endingSign
                };
            }
            else
            {
                return globalSignsOfSet;
            }
        }
        private static T GetElementValue<T>(XElement sourceElement, XName elementName, T defaultValue)
        {
            object value = default(T);
            try
            {
                string elementValue = sourceElement.Element(elementName).Value.ToString();

                if (typeof(T) == typeof(string))
                {
                    value = elementValue;
                }
                else if (typeof(T) == typeof(int))
                {
                    value = Int32.Parse(elementValue);
                }
                else if (typeof(T) == typeof(double))
                {
                    elementValue = elementValue.Replace(".", ",");
                    value = Double.Parse(elementValue);
                }
                else if (typeof(T) == typeof(DateTime))
                {
                    value = DateTime.Parse(elementValue);
                }
            }
            catch
            {
                return defaultValue;
            }
            return (T)(object)value;
        }
        private static T GetAttribute<T>(XElement sourceElement, XName attriubuteName, T defaultValue)
        {

            if (sourceElement.Attribute(attriubuteName) != null)
            {
                string attributeValue = sourceElement.Attribute(attriubuteName).Value.ToString();
                if (typeof(T) == typeof(bool))
                {
                    if (attributeValue == "true" || attributeValue == "1")
                        return (T)(object)true;
                    else
                        return (T)(object)false;
                }
                else if (typeof(T) == typeof(int))
                {
                    return (T)(object)Int32.Parse(attributeValue);
                }
                else if (typeof(T) == typeof(string))
                {
                    return (T)(object)attributeValue;
                }
                else if (typeof(T) == typeof(LogicalConnector))
                {
                    return (T)(object)GetLogicalConnector(attributeValue);
                }
                else
                    throw new NotImplementedException("Metoda GetAttribute nie została zaimplementowana dla tego typu T: " + (typeof(T).ToString()));
            }
            else
                return defaultValue;
        }
        private static void SetParametersInVisibilityConditions(List<Section> sections, List<Parameter> allParameters)
        {
            foreach (Section section in sections)
            {
                Condition visibilityCondition = section.VisibilityCondition as Condition;
                if (visibilityCondition != null)
                {
                    if (!SetParametersInCondition(visibilityCondition, allParameters, true))
                        section.VisibilityCondition = new TrueLogicalObject();
                }
                foreach (Parameter parameter in section)
                {
                    if (parameter.Sections.Count != 0)
                        SetParametersInVisibilityConditions(parameter.Sections, allParameters);
                }
            }
        }
        private static void ComputeVisibilityOfParameters(List<Section> sections, bool isHigherSectionVisible)
        {
            foreach (Section section in sections)
            {
                foreach (Parameter parameter in section)
                {
                    if(isHigherSectionVisible)
                    {
                        parameter.IsVisible = section.IsSectionVisible;
                        if (parameter.Sections.Count != 0)
                            ComputeVisibilityOfParameters(parameter.Sections, parameter.IsVisible);
                    }
                    else
                    {
                        parameter.IsVisible = false;
                        if (parameter.Sections.Count != 0)
                            ComputeVisibilityOfParameters(parameter.Sections, false);
                    }
                }
            }
        }
        private static List<Parameter> ListParameters(List<Section> sections)
        {
            List<Parameter> parameters = new List<Parameter>();
            foreach (Section section in sections)
            {
                foreach (Parameter parameter in section)
                {
                    parameters.Add(parameter);
                    if (parameter.Sections.Count != 0)
                        parameters.AddRange(ListParameters(parameter.Sections));
                }
            }
            return parameters;
        }
        private static bool SetParametersInCondition(Condition condition, List<Parameter> allParameters, bool isVisibilityCondition)
        {
            bool isFirstFound = false;
            bool isSecondFoundOrIrrelevant = false;

            foreach (Parameter parameter in allParameters)
            {
                if (!isFirstFound && parameter.Name == condition.FirstParameterName)
                {
                    condition.FirstParameter = parameter;
                    isFirstFound = true;
                }

                if (!isSecondFoundOrIrrelevant && condition.TypeOfSecondParameter == TypeOfSecondParameter.ParameterName)
                {
                    if (parameter.Name == condition.SecondParameterName)
                    {
                        condition.SecondParameter = parameter;
                        isSecondFoundOrIrrelevant = true;
                    }
                }
                else
                {
                    isSecondFoundOrIrrelevant = true;
                }
                if (isFirstFound && isSecondFoundOrIrrelevant)
                    break;
            }


            if (!isFirstFound || !isSecondFoundOrIrrelevant)
            {
                string incompleteParametersNames = "";
                if (!isFirstFound)
                {
                    incompleteParametersNames = condition.FirstParameterName;
                }
                if (!isSecondFoundOrIrrelevant)
                {
                    incompleteParametersNames += ", " + condition.SecondParameterName;
                }
                if (isVisibilityCondition)
                    MessageBox.Show("Parametr/y '" + incompleteParametersNames + "' nie występuje w zbiorze parametrów.\nSekcja będzie zawsze wyświetlana.");
                //MessageBox.Show("Parametr '" + incompleteParametersNames + "' nie występuje w zbiorze parametrów.\nSekcja zawsze będzie widoczna.");
                else
                    MessageBox.Show("Parametr/y '" + incompleteParametersNames + "' nie występuje w zbiorze parametrów.\nWarunek poprawności zawierający ten parametr zostanie zastąpiony wartością neutralną ze względu łącznik logiczny.");
                //MessageBox.Show("Parametr '" + incompleteParametersNames + "' nie występuje w zbiorze parametrów.\nWarunek poprawności zawierający ten parametr zostanie zastąpiony wartością neutralną ze względu łącznik logiczny.");
                return false;
            }
            return true;
        }
        private static bool SetParametersInCorrectnessConditions(LogicalObject logicalObject, List<Parameter> allParameters)
        {
            if (logicalObject as ConditionalSection != null)
            {
                List<LogicalObject> neutralConditions = new List<LogicalObject>();
                List<Condition> badConditions = new List<Condition>();
                ConditionalSection section = logicalObject as ConditionalSection;
                foreach (LogicalObject logicObject in section.Subsections)
                {
                    if (!SetParametersInCorrectnessConditions(logicObject, allParameters))
                    {
                        Condition badCondition = logicObject as Condition;
                        if (badCondition != null)
                        {
                            badConditions.Add(badCondition);
                            if (section.LogicalConnector == LogicalConnector.AND)
                                neutralConditions.Add(new TrueLogicalObject());
                            else if (section.LogicalConnector == LogicalConnector.OR)
                                neutralConditions.Add(new FalseLogicalObject());
                        }
                    }
                }
                foreach (Condition badCondition in badConditions)
                    section.Subsections.Remove(badCondition);
                section.Subsections.AddRange(neutralConditions);
            }
            else if (logicalObject as Condition != null)
            {
                return SetParametersInCondition(logicalObject as Condition, allParameters, false);
            }
            return true;
        }
        private static void AddRegExConditions(ConditionalSection correctnessConditions, List<Parameter> allParameters)
        {
            foreach (Parameter parameter in allParameters)
            {
                TextParameter textParameter = parameter as TextParameter;
                if (textParameter != null)
                {
                    if (textParameter.RegularExpression != default(string))
                    {
                        correctnessConditions.Subsections.Add(new Condition(textParameter.Name, TypeOfClause.RegEx, default(string), textParameter.RegularExpression, null, TypeOfSecondParameter.Value));
                    }
                }
            }

        }
        private static void CheckConditionsClausesTypesInCorrectness(ConditionalSection conditionalSection)
        {
            List<LogicalObject> toRemoveObjects = new List<LogicalObject>();
            List<LogicalObject> replacementObjects = new List<LogicalObject>();
            foreach (LogicalObject logicalObject in conditionalSection.Subsections)
            {
                Condition condition = logicalObject as Condition;
                if(condition != null)
                {
                    try
                    {
                        bool unimportant = condition.LogicalValue;
                    }
                    catch (InapplicableConditionException ice)
                    {
                        MessageBox.Show("Warunek poprawności zwrócił poniższy wyjątek. Zastępuje wartością neutralną.\n\n" + ice.Message, "Błędny warunek poprawności");
                        toRemoveObjects.Add(condition);
                        switch(conditionalSection.LogicalConnector)
                        {
                            case LogicalConnector.AND:
                                replacementObjects.Add(new TrueLogicalObject());
                                break;
                            case LogicalConnector.OR:
                                replacementObjects.Add(new FalseLogicalObject());
                                break;
                            default:
                                break;
                        }
                    }
                }
                ConditionalSection conditionalSubsection = logicalObject as ConditionalSection;
                if (conditionalSubsection != null)
                    CheckConditionsClausesTypesInCorrectness(conditionalSubsection);
            }
            foreach (LogicalObject logicalObject in toRemoveObjects)
                conditionalSection.Subsections.Remove(logicalObject);
            conditionalSection.Subsections.AddRange(replacementObjects);
            
        }
        private static void CheckConditionsClauseTypeInVisibilityConditions(List<Section> sections)
        {
            foreach (Section section in sections)
            {
                if (section.VisibilityCondition != null)
                {
                    try
                    {
                        bool unimportant = section.VisibilityCondition.LogicalValue;
                    }
                    catch (InapplicableConditionException ice)
                    {
                        MessageBox.Show("Warunek widoczności zwrócił poniższy wyjątek. Sekcja będzie zawsze wyświetlana.\n\n" + ice.Message, "Błędny warunek widoczności");
                        section.VisibilityCondition = new TrueLogicalObject();
                    }
                }
                foreach(Parameter paramter in section.Parameters)
                {
                    if(paramter.Sections != null)
                    {
                        CheckConditionsClauseTypeInVisibilityConditions(paramter.Sections);
                    }
                }
            }
        }
        public static void Save(string filePath, ParametersConfigurator model, ViewModel.SavingType savingType)
        {
            string content = "";
            content = GenerateSectionText(content, model.Sections, savingType);

            File.WriteAllText(filePath, content);
        }
        private static string GenerateSectionText(string currentContent, List<Section> sections, ViewModel.SavingType savingType)
        {
            foreach (Section section in sections)
            {
                if (section.VisibilityCondition.LogicalValue)
                {
                    foreach (Parameter parameter in section)
                    {
                        currentContent += parameter.ToFileSaveTxt(savingType);
                        if (parameter.Sections != null)
                            currentContent = GenerateSectionText(currentContent, parameter.Sections, savingType);
                    }
                }
            }
            return currentContent;
        }
    }
}
