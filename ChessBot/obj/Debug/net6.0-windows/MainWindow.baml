   M S B A M L   `   `   `  ����C  ?ChessBot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:ChessBotChessBot  5   	       ChessBot.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:ChessBot     5   	     � WindowStartupLocation$  CenterScreen=�5   	    � Title$ 
MainWindow��5	   	   $	��400��6   $	��400��6)   .��5
      �  � RowDefinitions 5   
   �� 5      �� 5      �� 5      .+�5   
   �� -   5         ��Name$ 
btnPlayBot��5   6   $��0��6   $	��135��6   $��40��6*   $��Play vs. Bot��6J   $��Center=�5      $��Center=�6,   $��0q�5      ��5      �� 5       �� 
TargetType" ��6   .�5      ��  �� Property � �� �� Value
 �����63   �� 5       � �� 5      �� 5       �� 
TargetType" ��6.   .R�5   "   �� #��z��6)   ._�5   &   �� $��Center=�67   $��Center=�6T   5   #   5      5       �� Triggers 5       P� 5!      	 P� Property	 �@J�IsMouseOver
 P� Value$

 True��69   . �5"      ��  � ��
 ��  �6;   �� 5#       � ��
 �����6;   5$      5&      5(      �� 5)   
   -   5        $ btnExit��5)   6   $��1��6   $	��135��6   $��40��6*   $
��Exit��6G   $��Center=�5*      $��Center=�6,   $��0q�5+      ��5,      �� 5-      " ��6   .�5.      ��  � ��
 �����63   �� 5/       � �� 50      �� 51      " ��6.   .R�52   "   �� #��z��6)   ._�53   &   �� $��Center=�67   $��Center=�6T   54   #   55      57       58      P� 59      	 �@J�IsMouseOver$

 True��69   . �5:      ��  � ��
 �  ��6;   �� 5;       � ��
 �����6;   5<      5>      5@      5A      5B      