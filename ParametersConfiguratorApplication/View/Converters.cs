using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace ParametersConfiguratorApplication.View
{
    public class VisibilityOfDescription : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targettype, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class MaxNumberOfValueToAsterix : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int maxNumberOfValue = (int)value;
            if (maxNumberOfValue == Int32.MaxValue)
            {
                return "*";
            }
            return maxNumberOfValue.ToString();
        }

        public object ConvertBack(object value, Type targettype, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ParameterUsageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Model.ParameterUsageType)value == Model.ParameterUsageType.Use && (string)parameter == "rbUse")
                return true;
            if ((Model.ParameterUsageType)value == Model.ParameterUsageType.UseEmpty && (string)parameter == "rbUseEmpty")
                return true;
            if ((Model.ParameterUsageType)value == Model.ParameterUsageType.Skip && (string)parameter == "rbSkip")
                return true;
            return false;
        }

        public object ConvertBack(object value, Type targettype, object parameter, CultureInfo culture)
        {
            if ((string)parameter == "rbUse")
                return Model.ParameterUsageType.Use;
            if ((string)parameter == "rbUseEmpty")
                return Model.ParameterUsageType.UseEmpty;
            if ((string)parameter == "rbSkip")
                return Model.ParameterUsageType.Skip;
            return null;
        }
    }
    public class SavingTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((ViewModel.SavingType)value == ViewModel.SavingType.None && (string)parameter == "rbNone")
                return true;
            if ((ViewModel.SavingType)value == ViewModel.SavingType.Quotation && (string)parameter == "rbQuotation")
                return true;
            if ((ViewModel.SavingType)value == ViewModel.SavingType.Apostrophe && (string)parameter == "rbApostrophe")
                return true;
            return false;
        }

        public object ConvertBack(object value, Type targettype, object parameter, CultureInfo culture)
        {
            if ((string)parameter == "rbNone")
                return ViewModel.SavingType.None;
            if ((string)parameter == "rbQuotation")
                return ViewModel.SavingType.Quotation;
            if ((string)parameter == "rbApostrophe")
                return ViewModel.SavingType.Apostrophe;
            return null;
        }
    }
    public class ParameterUsageToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Model.ParameterUsageType)value == Model.ParameterUsageType.Use)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targettype, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ParameterUsageToBrush : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Model.ParameterUsageType)value == Model.ParameterUsageType.Use)
                return Brushes.Black;
            return Brushes.DarkGray;
        }

        public object ConvertBack(object value, Type targettype, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ParameterUsageToBoolean : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Model.ParameterUsageType)value == Model.ParameterUsageType.Use)
                return true;
            return false;
        }

        public object ConvertBack(object value, Type targettype, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BooleanToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((string)parameter == "reverse")
                return (bool)value ? Visibility.Collapsed : Visibility.Visible;
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targettype, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class BooleanToBackground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Brushes.LightGoldenrodYellow : Brushes.LightCoral ;
        }

        public object ConvertBack(object value, Type targettype, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class EnumerationItemToEnabled : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)values[0])
                return true;
            else if (values[1] != DependencyProperty.UnsetValue && (bool)values[1])
                return false;
            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class BooleanToDateFormatString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "dd-MM-yyyy HH:mm" : "dd-MM-yyyy";
        }

        public object ConvertBack(object value, Type targettype, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class BooleanToDateFormat : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? DateTimeFormat.UniversalSortableDateTime : DateTimeFormat.ShortDate;
        }

        public object ConvertBack(object value, Type targettype, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class RemoveParameterValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
           
            if (values[0] != DependencyProperty.UnsetValue && values[1] != DependencyProperty.UnsetValue && values[0] != null && values[1] != null)
            {
                return new ViewModel.ParameterItemPair
                {
                    _parameter = values[0] as Model.Parameter,
                    _item = values[1]
                };
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

       //public class CurrentQuestionToButtonsVisibility : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        bool isCurrentQuestionLast = (bool)value;
    //        string button = (string)parameter;
    //        if (!isCurrentQuestionLast && button == "Next question")
    //            return Visibility.Visible;
    //        else if (isCurrentQuestionLast && button == "Show summary")
    //            return Visibility.Visible;
    //        else
    //            return Visibility.Collapsed;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //public class BooleanToMessageDialogImageIconConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        bool isValidate = (bool)value;
    //        if (isValidate)
    //            return MessageBoxImage.None;
    //        else
    //            return MessageBoxImage.Error;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}


    //public class BoolToCheckedConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        bool isAnswerSelected = (bool)value;
    //        if (isAnswerSelected)
    //            return ToggleButton.
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
