   M S B A M L   `   `   `  ����\  XParametersConfiguratorApplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null_4clr-namespace:ParametersConfiguratorApplication.View&ParametersConfiguratorApplication.View  5      8    2ParametersConfiguratorApplication.View.SummaryView   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Flocal4clr-namespace:ParametersConfiguratorApplication.View     5        � 	Resources  5	      @   :ParametersConfiguratorApplication.View.SavingTypeConverter   savingTypeConverter��%   &	          5
   
   6$   5      .��5      �� $��10q�6   .+�5   
   =   7ParametersConfiguratorApplication.View.MessageDialogBox  -   5          Name$ exitQuestion��5   !      Buttons$ YesNo=�67   6   0ParametersConfiguratorApplication.View.DialogBox   Caption$ Parameters Configurator��6G      
CommandYes 6i   �� *CloseApplicationCommand+>   8ParametersConfiguratorApplication.View.SaveFileDialogBox  5   
   -   5          Name$ saveFileDialogBox��5   "   $  Zapisz plik konfiguracyjny��6>   :   4ParametersConfiguratorApplication.View.FileDialogBox   Filter$< 6Pliki tekstowe (*.txt)|*.txt|Wszystkie pliki (*.*)|*.*��5   "      FilterIndex$ 0��6b   $��0, 1, 0, -1q�6r      CommandFileOk 5   c   �� *'%SaveParametersConfiguratorFileCommand+=   7ParametersConfiguratorApplication.View.CommandDialogBox	   CommandAfter	 5   "   �� *SavingSuccessCommand+5   r   �� 5   
   $��5q�6   .��5   
   �� $
��Wrap=�6   $��5q�6(   �� 5      �6   �� *SummaryContent+
 �� Mode$
 OneWay=�5      5      	� 5   
   $��1q�6    	� 	GroupName$ SavingTypeRb��6!    d� 	IsChecked 6:   �� *
SavingType+$
 TwoWay=� �� 	Converter# [   �� ConverterParameter$ rbNone��.��5      �� $��1q�6   $��Czyste wartości��6   5      	� 5   
   $��1q�6   $ SavingTypeRb��6!    6:   �� *
SavingType+$
 TwoWay=�# [  $ rbQuotation��.��5      �� $��1q�6   $"��Wartości w cudzysłowie (")��6   5      	� 5   
   $��1q�6   $ SavingTypeRb��6!    6:   �� *
SavingType+$
 TwoWay=�# [  $ rbApostrophe��.��5      �� $��0q�6   $!��Wartości w apostrofach (')��6   5      �� 5    
   $��5q�6   ��6   �� *Show+ �� ElementName$ saveFileDialogBox��.��5!      �� .�6   Zapisz plik6    5"      �� 5#   
   $��5q�6   $��ParametersViewModel��6y   ��6   �� $ MainWindowContentControl�� �� Path$# DataContext.ChangePageCommand�.��5$      �� .�6   Popraw parametry6%   5%      �� 5&   
   $��5q�6   $/��)Czy na pewno chcesz opuścić aplikację?��6O   ��6   �� *Show+$ exitQuestion��.��5'      �� .�6   Zamknij aplikację6&   5(      5)      5*      