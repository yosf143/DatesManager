   M S B A M L   `   `   `  ����   DatesMgr!    DatesMgr.AlertMessageWindow   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$/  )تواريخ قريبة الإنتهاء!�� � WindowStartupLocation$ CenterScreen=� � SizeToContent$ Width=�$��RightToLeft=�$	��700��$	��600��.��� .+�o� .@�m� -   $��
طباعة����  �� Orientation$ 
Horizontal=�$��0,0,0,0q�.+��� -   $��/Imgs/help.png��$��16��$��16���� $��0,10,0,0q�.+��� $.��(تواريخ قريبة الإنتهاء��$��16(�$
��Bold"�$��Center=�$��0,0,0,10q��� $�� ��$��0,0,0,10q�&   System.Windows.Controls.DataGrid  -     Name$ datagridAllProducts��   
IsReadOnly$
 True��   AutoGenerateColumns$ False��$	��200��$��0,0,0,10q�   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ إسم المنتج��1  +System.Windows.Controls.DataGridBoundColumn	   Binding	 �� *ProductName+  $! تاريخ الإنتهاء��	 �� *
ExpireDate+
 �� StringFormat$
 {0:yyyy-MM-dd}��  $ 
متبقي��	 �� *RemainingDays+   ElementStyle ��  �� 
TargetType" ��.���  �� Property � �� �� Value �� *RemainingDays+$
 
{0} يوم��  $ الوحدة��	 �� *Unit+  $ الكمية��	 �� *Qty+  $ سعر العرض��	 �� *DiscountPrice+  $ ملاحظات��	 �� *Notes+