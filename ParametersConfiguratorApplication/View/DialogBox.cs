using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ParametersConfiguratorApplication.View
{
    public abstract class DialogBox : FrameworkElement, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string nazwaWłasności)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nazwaWłasności));
        }
        #endregion

        protected Action<object> execute = null;

        public string Caption { get; set; }

        protected ICommand show;

        public virtual ICommand Show
        {
            get
            {
                if (show == null) show = new RelayCommand(execute);
                return show;
            }
        }
    }

    public class SimpleMessageDialogBox : DialogBox
    {
        public SimpleMessageDialogBox()
        {
            execute =
                o =>
                {
                    MessageBox.Show((string)o, Caption);
                };
        }
    }

    public abstract class CommandDialogBox : DialogBox
    {
        public override ICommand Show
        {
            get
            {
                if (show == null) show = new RelayCommand(
                    o =>
                    {
                        ExecuteCommand(CommandBefore, CommandParameter);
                        execute(o);
                        ExecuteCommand(CommandAfter, CommandParameter);
                    });
                return show;
            }
        }

        public static DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(CommandDialogBox));

        public object CommandParameter
        {
            get
            {
                return GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

        protected static void ExecuteCommand(ICommand command, object commandParameter)
        {
            if (command != null)
                if (command.CanExecute(commandParameter))
                    command.Execute(commandParameter);
        }

        public static DependencyProperty CommandBeforeProperty = DependencyProperty.Register("CommandBefore", typeof(ICommand), typeof(CommandDialogBox));

        public ICommand CommandBefore
        {
            get
            {
                return (ICommand)GetValue(CommandBeforeProperty);
            }
            set
            {
                SetValue(CommandBeforeProperty, value);
            }
        }

        public static DependencyProperty CommandAfterProperty = DependencyProperty.Register("CommandAfter", typeof(ICommand), typeof(CommandDialogBox));

        public ICommand CommandAfter
        {
            get
            {
                return (ICommand)GetValue(CommandAfterProperty);
            }
            set
            {
                SetValue(CommandAfterProperty, value);
            }
        }

    }
    public class NotificationDialogBox : CommandDialogBox
    {
        public NotificationDialogBox()
        {
            execute =
                o =>
                {
                    MessageBox.Show((string)o, Caption, MessageBoxButton.OK, MessageBoxImage.Information);
                };
        }
    }

    public class MessageDialogBox : CommandDialogBox
    {
        public MessageBoxResult? LastResult { get; protected set; }
        public MessageBoxButton Buttons { get; set; } = MessageBoxButton.OK;
        public MessageBoxImage Icon { get; set; } = MessageBoxImage.None;


        public bool IsLastResultYes
        {
            get
            {
                if (!LastResult.HasValue) return false;
                return LastResult.Value == MessageBoxResult.Yes;
            }
        }
        public bool IsLastResultNo
        {
            get
            {
                if (!LastResult.HasValue) return false;
                return LastResult.Value == MessageBoxResult.No;
            }
        }
        public bool IsLastResultCancel
        {
            get
            {
                if (!LastResult.HasValue) return false;
                return LastResult.Value == MessageBoxResult.Cancel;
            }
        }
        public bool IsLastResultOK
        {
            get
            {
                if (!LastResult.HasValue) return false;
                return LastResult.Value == MessageBoxResult.OK;
            }
        }
        public MessageDialogBox()
        {
            execute = o =>
            {
                LastResult = MessageBox.Show((string)o, Caption, Buttons, Icon);
                OnPropertyChanged("LastResult");
                switch (LastResult)
                {
                    case MessageBoxResult.Yes:
                        OnPropertyChanged("IsLastResultYes");
                        ExecuteCommand(CommandYes, CommandParameter);
                        break;
                    case MessageBoxResult.No:
                        OnPropertyChanged("IsLastResultNo");
                        ExecuteCommand(CommandNo, CommandParameter);
                        break;
                    case MessageBoxResult.Cancel:
                        OnPropertyChanged("IsLastResultCancel");
                        ExecuteCommand(CommandCancel, CommandParameter);
                        break;
                    case MessageBoxResult.OK:
                        OnPropertyChanged("IsLastResultOK");
                        ExecuteCommand(CommandOK, CommandParameter);
                        break;
                }
            };
        }

        public static DependencyProperty CommandYesProperty = DependencyProperty.Register("CommandYes", typeof(ICommand), typeof(MessageDialogBox));
        public static DependencyProperty CommandNoProperty = DependencyProperty.Register("CommandNo", typeof(ICommand), typeof(MessageDialogBox));
        public static DependencyProperty CommandCancelProperty = DependencyProperty.Register("CommandCancel", typeof(ICommand), typeof(MessageDialogBox));
        public static DependencyProperty CommandOKProperty = DependencyProperty.Register("CommandOK", typeof(ICommand), typeof(MessageDialogBox));

        public ICommand CommandYes
        {
            get
            {
                return (ICommand)GetValue(CommandYesProperty);
            }
            set
            {
                SetValue(CommandYesProperty, value);
            }
        }

        public ICommand CommandNo
        {
            get
            {
                return (ICommand)GetValue(CommandNoProperty);
            }
            set
            {
                SetValue(CommandNoProperty, value);
            }
        }

        public ICommand CommandCancel
        {
            get
            {
                return (ICommand)GetValue(CommandCancelProperty);
            }
            set
            {
                SetValue(CommandCancelProperty, value);
            }
        }

        public ICommand CommandOK
        {
            get
            {
                return (ICommand)GetValue(CommandOKProperty);
            }
            set
            {
                SetValue(CommandOKProperty, value);
            }
        }
    }

    public class ConcitionalMessageDialogBox : MessageDialogBox
    {
        public static DependencyProperty IsDialogBypassedProperty = DependencyProperty.Register("IsDialogBypassed", typeof(bool), typeof(ConcitionalMessageDialogBox));

        public bool IsDialogBypassed
        {
            get
            {
                return (bool)GetValue(IsDialogBypassedProperty);
            }
            set
            {
                SetValue(IsDialogBypassedProperty, value);
            }
        }

        public MessageBoxResult DialogBypassButton { get; set; } = MessageBoxResult.None;

        public ConcitionalMessageDialogBox()
        {
            execute = o =>
            {
                MessageBoxResult result;
                if (!IsDialogBypassed)
                {
                    LastResult = MessageBox.Show((string)o, Caption, Buttons, Icon);
                    OnPropertyChanged("LastResult");
                    result = LastResult.Value;
                }
                else
                {
                    result = DialogBypassButton;
                }
                switch(result)
                {
                    case MessageBoxResult.Yes:
                        if (!IsDialogBypassed) OnPropertyChanged("IsLastResultYes");
                        ExecuteCommand(CommandYes, CommandParameter);
                        break;
                    case MessageBoxResult.No:
                        if (!IsDialogBypassed) OnPropertyChanged("IsLastResultNo");
                        ExecuteCommand(CommandNo, CommandParameter);
                        break;
                    case MessageBoxResult.Cancel:
                        if (!IsDialogBypassed) OnPropertyChanged("IsLastResultCancel");
                        ExecuteCommand(CommandCancel, CommandParameter);
                        break;
                    case MessageBoxResult.OK:
                        if (!IsDialogBypassed) OnPropertyChanged("IsLastResultOK");
                        ExecuteCommand(CommandOK, CommandParameter);
                        break;
                }
            };
        }
    }

    public abstract class FileDialogBox : CommandDialogBox
    {
        public bool? FileDialogResult { get; protected set; }
        public string FilePath { get; set; }
        public string Filter { get; set; }
        public int FilterIndex { get; set; }
        public string DefaultExt { get; set; }

        protected Microsoft.Win32.FileDialog fileDialog = null;
        protected FileDialogBox()
        {
            execute =
            o =>
            {
                fileDialog.Title = Caption;
                fileDialog.Filter = Filter;
                fileDialog.FilterIndex = FilterIndex;
                fileDialog.DefaultExt = DefaultExt;
                string ścieżkaPliku = "";
                if (FilePath != null) ścieżkaPliku = FilePath; else FilePath = "";
                if (o != null) ścieżkaPliku = (string)o;
                if (!string.IsNullOrWhiteSpace(ścieżkaPliku))
                {
                    fileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(ścieżkaPliku);
                    fileDialog.FileName = System.IO.Path.GetFileName(ścieżkaPliku);
                }
                FileDialogResult = fileDialog.ShowDialog();
                OnPropertyChanged("FileDialogResult");
                if (FileDialogResult.HasValue && FileDialogResult.Value)
                {
                    FilePath = fileDialog.FileName;
                    OnPropertyChanged("FilePath");
                    ExecuteCommand(CommandFileOk, FilePath);
                };
            };
        }
        public static DependencyProperty CommandFileOkProperty = DependencyProperty.Register("CommandFileOk", typeof(ICommand), typeof(FileDialogBox));

        public ICommand CommandFileOk
        {
            get
            {
                return (ICommand)GetValue(CommandFileOkProperty);
            }
            set
            {
                SetValue(CommandFileOkProperty, value);
            }
        }
    }
    public class OpenFileDialogBox : FileDialogBox
    {
        public OpenFileDialogBox()
        {
            fileDialog = new Microsoft.Win32.OpenFileDialog();
        }
    }
    public class SaveFileDialogBox : FileDialogBox
    {
        public SaveFileDialogBox()
        {
            fileDialog = new Microsoft.Win32.SaveFileDialog();
        }
    }
}
