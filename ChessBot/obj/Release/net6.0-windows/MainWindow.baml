   M S B A M L   `   `   `  ���� C  ?ChessBot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:ChessBotChessBot      ChessBot.MainWindow   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     (localclr-namespace:ChessBot       � WindowStartupLocation$  CenterScreen=� � Title$ 
MainWindow��$	��400��$	��400��.���  � RowDefinitions �� �� �� .+��� -    ��Name$ 
btnPlayBot��$��0��$	��135��$��40��$��Play vs. Bot��$��Center=�$��Center=�$��0q�����  �� 
TargetType" ��.���  �� Property � �� �� Value
 �������  � �� ��  �� 
TargetType" ��.R��� #��z��._��� $��Center=�$��Center=� �� Triggers P� 	 P� Property	 �@J�IsMouseOver
 P� Value$

 True��. ���  � ��
 ��  ���  � ��
 ������� -   $ btnExit��$��1��$	��135��$��40��$
��Exit��$��Center=�$��Center=�$��0q����� " ��.���  � ��
 �������  � �� �� " ��.R��� #��z��._��� $��Center=�$��Center=� P� 	 �@J�IsMouseOver$

 True��. ���  � ��
 �  ����  � ��
 �����