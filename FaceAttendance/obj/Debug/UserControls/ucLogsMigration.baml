   M S B A M L   `   `   `  ����I  EFaceAttendance, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:FaceAttendance.UserControlsFaceAttendance.UserControls  5      1    +FaceAttendance.UserControls.ucLogsMigration   5      n jSAPBusinessObjects.WPF.ViewerShared, Version=13.0.4000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ;local)clr-namespace:FaceAttendance.UserControls     5      -   5        .��5
      � -   5          �Name$  MainGrid��5
      .+�5   
   �� -   5         ��Name$ MainStackPanel��5      $	��Top=�6-   $��0,0,0,0q�6B   .+�5      �� .+�5      �� $��0,0,0,0q�5      $��Logs Migration��5      $��20(�5      ���#2b7dbc5      �� 5      
���   �$��2��6/   6<   � 5       � ColumnDefinitions 5      �� 5      $
��0.5* �6+   �� 5      $��0.01* �6+   �� 5      $��* �6+   .+�5      �� 	 �� X1$ 0��6   	 �� Y1$ 0��6"   	 �� X2$ 0��6)   
�������$��2��6>   $	��Top=�5      $��Center=�63   $��1��5      	 �� Y2 5      �� 5       �� Path$ ActualHeight�6&    �� ElementName$ SourceStackPanel��6:   5      �� 5       -   5        $��SourceStackPanel��5    1   $��0��6!   .+�5!      �� $��0,0,0,0q�5"      $��Source Database��5#      $��20(�5$      ���#2b7dbc5%      � 5&      $��0, 50, 0, 0q�6    5'      �� 5(   "   $
��0.5* �63   �� 5)   "   $��* �63   .+�5+      �� $��0��5,      $��0,0,0,0q�5-      $��
Database :��5.      $��20(�5/      ���#2b7dbc50      �� 5=      -   5        $��
txt_search��5?   !   $��1��5>   !   $	��150��5@   !   $��30��5A   !   $
��Left=�5B   !   	 �� 
IsReadOnly	 . $��AASFF��5D   !   $��15(�5E   !   $	��5,5q�5F   !   
������ 5G   !   5J      � 5K      -   5        $��
SourceGrid��5K       5L      �� 5M   "   $
��0.5* �63   �� 5N   "   $��* �63   .+�5P      �� $��0��5Q      $��0,0,0,0q�5R      $��Logs Table :��5S      $��20(�5T      ���#2b7dbc5U      �� 5W      $��1��5X   !   $	��150��5Y   !   $��30��5Z   !   $
��Left=�5[   !   	 . $��tblLog��5]   !   $��15(�5^   !   $��Center=�5_   !   $��
5, 0, 0, 0q�5`   !   
������ 5a   !   5d      � 5e      
������ ��5f      �� 5g   "   $ ActualHeight�6*   $ 
SourceGrid��6>   5i      � 5j      
������ ��5k      �� 5l   "   $ ActualHeight�6*   $ 
SourceGrid��6>   5n      � 5o      
������ ��5p      �� 5q   "   $ ActualHeight�6*   $ 
SourceGrid��6>   5s      � 5t      
������ $��0 10 0 0q�68   ��5u      �� 5v   "   $ ActualHeight�6*   $ 
SourceGrid��6>   .+�5x      �� $��0��5y      $��0,0,0,0q�5z      $$��---------- Bindings ----------��5{      $��20(�5|      $��Center=�5}   !   ���#2b7dbc5~      5      �� 5�      $ 0��6   $ 0��6&   $ 0��6-   
�������$��2��6B   $	��Top=�5�       5�      �� 5�   "   $ ActualWidth�6*   $ 
SourceGrid��6=   5�      � 5�      
������ $��0 10 0 0q�68   ��5�      �� 5�   "   $ ActualHeight�6*   $ 
SourceGrid��6>   .+�5�      �� $��0��5�      $��0,0,0,0q�5�      $
��[Id]��5�      $��20(�5�      $��Center=�5�   !   ���#2b7dbc5�      5�      � 5�      
������ $��0 10 0 0q�68   ��5�      �� 5�   "   $ ActualHeight�6*   $ 
SourceGrid��6>   .+�5�      �� $��0��5�      $��0,0,0,0q�5�      $��[TMchNo]��5�      $��20(�5�      $��Center=�5�   !   ���#2b7dbc5�      5�      � 5�      
������ $��0 10 0 0q�68   ��5�      �� 5�   "   $ ActualHeight�6*   $ 
SourceGrid��6>   .+�5�      �� $��0��5�      $��0,0,0,0q�5�      $��
[EnrollNo]��5�      $��20(�5�      $��Center=�5�   !   ���#2b7dbc5�      5�      � 5�      
������ $��0 10 0 0q�68   ��5�      �� 5�   "   $ ActualHeight�6*   $ 
SourceGrid��6>   .+�5�      �� $��0��5�      $��0,0,0,0q�5�      $��[EMchNo]��5�      $��20(�5�      $��Center=�5�   !   ���#2b7dbc5�      5�      � 5�      
������ $��0 10 0 0q�68   ��5�      �� 5�   "   $ ActualHeight�6*   $ 
SourceGrid��6>   .+�5�      �� $��0��5�      $��0,0,0,0q�5�      $��[InOut]��5�      $��20(�5�      $��Center=�5�   !   ���#2b7dbc5�      5�      � 5�      
������ $��0 10 0 0q�68   ��5�      �� 5�   "   $ ActualHeight�6*   $ 
SourceGrid��6>   .+�5�      �� $��0��5�      $��0,0,0,0q�5�      $��
[VeriMode]��5�      $��20(�5�      $��Center=�5�   !   ���#2b7dbc5�      5�      � 5�      
������ $��0 10 0 0q�68   ��5�      �� 5�   "   $ ActualHeight�6*   $ 
SourceGrid��6>   .+�5�      �� $��0��5�      $��0,0,0,0q�5�      $��
[DateTime]��5�      $��20(�5�      $��Center=�5�   !   ���#2b7dbc5�      5�      5�      �� 5�      $��2��6!   .+�5�      �� $��0,0,0,0q�5�      $��Destination Database��5�      $��20(�5�      ���#2b7dbc5�      �� 5�      $��	0,10,0,10q�5�      $
��Left=�5�      $	��Top=�5�      
 �� Orientation$
 
Horizontal=�5�      .+�5�      � -   5        $��50��5�   !   $��30��5�   !   $��0,0,10,0q�5�   !    � Cursor$
 Hand��5�   !   |�.  � ToolTip$ Create New Connection��5�   !   .+�5�   "   �� $��50��5�   %   $��Right=�5�   %    
  VISION#��[  5�   %   $��1,0,0,1q�5�   %    �� CornerRadius$ 5��5�   %   ._�5�   &   �� $��60��5�   )     HIK#��[ 5�   )   $��0,1,1,0q�5�   )   $ 10��5�   )   ��5�   &   �� 5�   *    � MappingMode$ RelativeToBoundingBox=�6>    �� 
StartPoint$ 0.5,0&�6b    �� EndPoint$ 0.5,1&�6u   .��5�   .   �   	RoundBack � Color# [ 6;   5�   +    J� Effect 5  &   3  -System.Windows.Media.Effects.DropShadowEffect  5  *     VISIONC   Color# [ 6;   5  #   �� 5  "   -   5        $��AddDev��5  %   $��50��5  %   $��30��5  %   $��3,0,3,0q�5	  %   $��Center=�5
  %   $��Bottom=�5  %   ��5  &   �� 5  *    �� 	UriSource$ ..\Images\add.png?�66   5  #   5     � 5     -	   5        $��ModDev��5  !   $��50��5  !   $��30��5  !   $
 Hand��5  !   |�. $ Modify Connection��5  !   .+�5  "   �� #��[  5  %   $��1,0,0,1q�5  %   $ 10��5  %   ._�5  &   �� $��60��5  )   #��[ 5   )   $��0,1,1,0q�5!  )   $ 5��5"  )   ��5#  &   �� 5$  *   $ RelativeToBoundingBox=�6>   $ 0.5,0&�6b   $ 0.5,1&�6u   .��5%  .   � # [ 6;   5&  +    5(  &     5)  *     HIKC# [ 6;   5+  #   �� 5,  "   -
   5        $��Modify��5-  %   $��50��5.  %   $��30��5/  %   $��3,0,3,0q�50  %   $��Center=�51  %   $��Bottom=�52  %   ��53  &   �� 54  *   $ ..\Images\mode_edit.png?�66   56  #   57     � 58     -   5        $��50��59  !   $��30��5:  !   $��10,0,0,0q�5;  !   $
 Hand��5<  !   |�. $ Delete Connection��5?  !   .+�5A  "   �� #��[  5B  %   $��1,0,0,1q�5C  %   $ 5��5D  %   ._�5E  &   �� $��60��5F  )   #��[ 5G  )   $��0,1,1,0q�5H  )   $ 10��5I  )   ��5J  &   �� 5K  *   $ RelativeToBoundingBox=�6>   $ 0.5,0&�6b   $ 0.5,1&�6u   .��5L  .   � # [ 6;   5M  +    5O  &     5P  *    
 CloseC# [ 6;   5R  #   �� 5S  "   -   5        $��Delete��5T  %   $��50��5U  %   $��30��5V  %   $��3,0,3,0q�5W  %   $��Center=�5X  %   $��Bottom=�5Y  %   ��5Z  &   �� 5[  *   $ ..\Images\Delete.png?�66   5]  #   5g     5i     � 5j      5k     �� 5l  "   $��* �63   �� 5m  "   $��5* �63   .+�5o     �� $��0,0,0,0q�5p  %   $��Server :��5q  %   $��20(�5r  %   ���#2b7dbc5s  %   � 5t     $��1��6#   .+�5v  "   �� -   5        $��txtOverlayServer��5w  )   $	��200��5x  )   $��30��5y  )   ���#777A7D$
��Left=�5{  )   	 . $��Enter Sql Server��5}  )   $��15(�5~  )   $	��5,0q�5  )   $��Center=�5�  )   
������ 5�  )   �� 5�  "   -   5        $��	txtServer��5�  )   $	��200��5�  )   $��30��5�  )   $��15(�5�  )   $��5,0,0,0q�5�  )   {�.  $
��Left=�5�  )   
������ $��Center=�5�  )   5�  )   5�     5�     � 5�      5�     �� 5�  "   $��* �63   �� 5�  "   $��5* �63   .+�5�     �� $��0,0,0,0q�5�  %   $��	User ID :��5�  %   $��20(�5�  %   $
��Left=�5�  %   ���#2b7dbc5�  %   � 5�     $��1��6#   .+�5�  "   �� -   5        $��txtOverlayUserID��5�  )   $��1��5�  )   $	��200��5�  )   $��30��5�  )   ���#777A7D$
��Left=�5�  )   	 . $��Enter Sql User��5�  )   $��15(�5�  )   $	��5,0q�5�  )   $��Center=�5�  )   
������ 5�  )   �� 5�  "   -   5        $��	txtUserID��5�  )   $	��200��5�  )   $��1��5�  )   $��15(�5�  )   $��30��5�  )   $��5,0,0,0q�5�  )   {�.  $��Center=�5�  )   $
��Left=�5�  )   
������ 5�  )   5�     5�     � 5�      5�     �� 5�  "   $��* �63   �� 5�  "   $��5* �63   .+�5�     �� $��0,0,0,0q�5�  %   $��
Password :��5�  %   $��20(�5�  %   $
��Left=�5�  %   ���#2b7dbc5�  %   � 5�     $��1��6#   .+�5�  "   �� -   5        $��txtOverlayPwd��5�  )   $��1��5�  )   $	��200��5�  )   $��30��5�  )   ���#777A7D$
��Left=�5�  )   	 . $��Enter Sql Pwd��5�  )   $��15(�5�  )   $	��5,0q�5�  )   $��Center=�5�  )   
������ 5�  )   �� 5�  "   -   5        $��txtPwd��5�  )   $	��200��5�  )   $��1��5�  )   $��30��5�  )   $��15(�5�  )   {�.  $��Center=�5�  )   $��5,0,0,0q�5�  )   $
��Left=�5�  )   
������ 5�  )   5�     5�     � 5�      5�     �� 5�  "   $��* �63   �� 5�  "   $��5* �63   .+�5�     �� $��0,0,0,0q�5�  %   $��	DB Name :��5�  %   $��20(�5�  %   $
��Left=�5�  %   ���#2b7dbc5�  %   � 5�     $��1��6#   .+�5�  "   �� -   5        $��txtOverlayDBName��5�  )   $��1��5�  )   $	��200��5�  )   $��30��5�  )   ���#777A7D$
��Left=�5�  )   	 . $��Enter Database Name��5   )   $��15(�5  )   $	��5,0q�5  )   $��Center=�5  )   
������ 5  )   �� 5  "   -   5        $��	txtDBName��5  )   $	��200��5  )   $��1��5	  )   $��30��5
  )   {�.  $��15(�5  )   $��Center=�5  )   $��5,0,0,0q�5  )   $
��Left=�5  )   
������ 5  )   5     � 5     -   5        $��1��5  !   $��30��5  !   $��30��5  !   $
��Left=�5  !   $��	200 0 0 0q�5  !   .+�5  "   �� -   5        $��downUser��5  %   $��30��5  %   $��30��5   %   $��0,0,0,0q�5!  %   $ Reset Fields��5$  %   ��5%  &   �� 5&  *   $ ..\Images\refresh.png?�66   5(  #   5)     5*     � 5+     -   5        $��SourceGrid2��5+      5,     �� 5-  "   $��* �63   �� 5.  "   $��5* �63   .+�51     �� -   5        $��btnSaveConnection��53  %   $��1��52  !   $
��20,9q�54  %   $
��Left=�55  %   $��57 0 0 0q�57  %   $��Save Connection��58  %   $
 Hand��59  %   |�. $��14(�5;  %   {�.    3DButtonStyle#��[ 5=  %   $��6��5>  %   �� 5?     -   5        $��btnUpdateConnection��5A  %   $��1��5@  !   $
��20,9q�5B  %   $
��Left=�5C  %   $��40 0 0 0q�5E  %   $��Update Connection��5F  %   $
 Hand��5G  %   $y�	Collapsed=�5H  %   |�. $��14(�5J  %   #��[ 5K  %   $��6��5L  %   5N     � 5O     $��0 10 0 0q�6    5P     �� 5Q  "   $��* �63   �� 5R  "   $��5* �63   .+�5T     �� $��0,0,0,0q�5U  %   $��Select Table :��5V  %   $��20(�5W  %   ���#2b7dbc5X  %   � 5Y     $��1��6#   .+�5[  "   �� -   5        $��	cmbTables��5\  %   $	��200��5]  %   $��30��5^  %   ���#777A7D$
��Left=�5`  %    �� 
IsReadOnly . $��15(�5b  %   $	��5,0q�5c  %   $��Center=�5d  %    �� SelectedIndex$ 0��5f  %   .@�5g  &   �� $��--- Select Table ---��63   {�.  6R   5h  #   5i     5j     �� 5k     $ 0��6   $ 0��6&   $ 0��6-   
�������$��2��6B   $	��Top=�5l      5n     �� 5o  "   $ ActualWidth�6*   $ SourceGrid2��6=   5q     � 5r     $��0 10 0 0q�6    5s     �� 5t  "   $��* �63   �� 5u  "   $��5* �63   .+�5w     �� $��0,0,0,0q�5x  %   $��Column :��5y  %   $��20(�5z  %   ���#2b7dbc5{  %   � 5|     $��1��6#   .+�5~  "   �� -   5        $��cmbId��5  %   $	��200��5�  %   $��30��5�  %   ���#777A7D$
��Left=�5�  %    . $��15(�5�  %   $	��5,0q�5�  %   $��Center=�5�  %   $ 0��5�  %   .@�5�  &   �� $��--- Select Column ---��63   {�.  6S   5�  #   5�     5�     � 5�     $��0 10 0 0q�6    5�     �� 5�  "   $��* �63   �� 5�  "   $��5* �63   .+�5�     �� $��0,0,0,0q�5�  %   $��Column :��5�  %   $��20(�5�  %   ���#2b7dbc5�  %   � 5�     $��1��6#   .+�5�  "   �� -   5        $��	cmbTMchNo��5�  %   $	��200��5�  %   $��30��5�  %   ���#777A7D$
��Left=�5�  %    . $��15(�5�  %   $	��5,0q�5�  %   $��Center=�5�  %   $ 0��5�  %   .@�5�  &   �� $��--- Select Column ---��63   {�.  6S   5�  #   5�     5�     � 5�     $��0 10 0 0q�6    5�     �� 5�  "   $��* �63   �� 5�  "   $��5* �63   .+�5�     �� $��0,0,0,0q�5�  %   $��Column :��5�  %   $��20(�5�  %   ���#2b7dbc5�  %   � 5�     $��1��6#   .+�5�  "   �� -   5        $��cmbEnrollNo��5�  %   $	��200��5�  %   $��30��5�  %   ���#777A7D$
��Left=�5�  %    . $��15(�5�  %   $	��5,0q�5�  %   $��Center=�5�  %   $ 0��5�  %   .@�5�  &   �� $��--- Select Column ---��63   {�.  6S   5�  #   5�     5�     � 5�     $��0 10 0 0q�6    5�     �� 5�  "   $��* �63   �� 5�  "   $��5* �63   .+�5�     �� $��0,0,0,0q�5�  %   $��Column :��5�  %   $��20(�5�  %   ���#2b7dbc5�  %   � 5�     $��1��6#   .+�5�  "   �� -   5        $��	cmbEMchNo��5�  %   $	��200��5�  %   $��30��5�  %   ���#777A7D$
��Left=�5�  %    . $��15(�5�  %   $	��5,0q�5�  %   $��Center=�5�  %   $ 0��5�  %   .@�5�  &   �� $��--- Select Column ---��63   {�.  6S   5�  #   5�     5�     � 5�     $��0 10 0 0q�6    5�     �� 5�  "   $��* �63   �� 5�  "   $��5* �63   .+�5�     �� $��0,0,0,0q�5�  %   $��Column :��5�  %   $��20(�5�  %   ���#2b7dbc5�  %   � 5�     $��1��6#   .+�5�  "   �� -   5        $��cmbInOut��5�  %   $	��200��5�  %   $��30��5�  %   ���#777A7D$
��Left=�5�  %    . $��15(�5�  %   $	��5,0q�5�  %   $��Center=�5�  %   $ 0��5�  %   .@�5�  &   �� $��--- Select Column ---��63   {�.  6S   5�  #   5�     5�     � 5�     $��0 10 0 0q�6    5�     �� 5   "   $��* �63   �� 5  "   $��5* �63   .+�5     �� -    5        $��SourceColumnWidth��5  %   $��0,0,0,0q�5  %   $��Column :��5  %   $��20(�5  %   ���#2b7dbc5  %   � 5	     $��1��6#   .+�5  "   �� -!   5        $��cmbVeriMode��5  %   $	��200��5  %   $��30��5  %   ���#777A7D$
��Left=�5  %    . $��15(�5  %   $	��5,0q�5  %   $��Center=�5  %   $ 0��5  %   .@�5  &   �� $��--- Select Column ---��63   {�.  6S   5  #   5     5     � 5     $��0 10 0 0q�6    5     �� 5  "   ��5  &   �� 5  *   $ ActualWidth�62   $ SourceColumnWidth��6E   5!  #   �� 5"  "   $	��200 �63   �� 5#  "   $��* �63   .+�5%     �� $��0,0,0,0q�5&  %   $��Column :��5'  %   $��20(�5(  %   ���#2b7dbc5*     � 5+     $��1��6#   .+�5-  "   �� -"   5        $��cmbDateTime��5.  %   $	��200��5/  %   $��30��50  %   ���#777A7D$
��Left=�52  %    . $��15(�54  %   $	��5,0q�56  %   $��Center=�57  %   $ 0��58  %   .@�59  &   �� $��--- Select Column ---��63   {�.  6S   5:  #   5;     �� 5<     $��2��6)   $
 
Horizontal=�69   5K     5L     � 5M     $��0 10 0 0q�6    5N     �� 5O  "   $��* �63   �� 5P  "   $
��6.5* �63   .+�5S     � $��1��6#   .+�5T  "   �� -#   5        $��btnSync��5U  %   $
��20,9q�5V  %   $
��Left=�5W  %   $��	140 0 0 0q�5Y  %   $��
Start Sync��5Z  %   $
 Hand��5[  %   |�. $��14(�5]  %   {�. #��[ 5_  %   $��6��5`  %   �� 5a  "   -$   5        $��btnSaveBinding��5c  %   $��1��5b  %   $
��20,9q�5d  %   $
��Left=�5e  %   $��0 0 0 0q�5g  %   $��Save Bindings��5h  %   $
 Hand��5i  %   |�. $��14(�5k  %   {�. #��[ 5m  %   $��6��5n  %   5o     5p     5(     5)     5*     5+     5,     5-     