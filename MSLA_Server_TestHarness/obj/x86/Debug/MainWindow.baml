   M S B A M L   `   `   `  ����  MSLA_Server_TestHarness(    "MSLA_Server_TestHarness.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  
MainWindow��5   	   $	��350��6   $	��525��6)   .��5      � .+�5   
   &   System.Windows.Controls.DataGrid  -   5        $��	dataGrid1��5   j      AutoGenerateColumns .  $	��200��6/   $
��Left=�6<   $��	10,10,0,0q�6W   $	��Top=�6{   $	��481��6�      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ 
Account ID��6G   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6%   �� *fldAccount_ID+6G     5	      $
 Date��6A      Width5  /System.Windows.Controls.DataGridLengthConverter$ 80 6O    6%   �� *	fldDate+6O     5
      $ 
Voucher No��6G   $	 130 6[    6%   �� *fldVoucher_ID+6[     5      $ Debits��6E   $	 130 6U    6%   �� *fldDebitAmt+6U     5      $ Credits��6F   $	 130 6W    6%   �� *fldCreditAmt+6W   5      �� 5   
   -   5        $��cmdFetch��5   \   $��Fetch��6   $��23��6!   $
��Left=�6-   $��
10,220,0,0q�6H   $	��Top=�6l   $��75��6�   6�   �� 5   
   -   5        $��textBox1��5   N   $��23��6   $
��Left=�6   $��113,219,0,0q�69   $	��Top=�6^   $	��120��6v   5      5      