   M S B A M L   `   `   `  ���� G  CPS4wpfDumper, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:PS4wpfDumperPS4wpfDumper      PS4wpfDumper.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     ,localclr-namespace:PS4wpfDumper     -     �Name$  
PS4_Dumper�� � Title$ 
PS4 Dumper�� � SizeToContent$ WidthAndHeight=�$��788.448�� � Icon$ libs/BigMac.ico��.��� $��0,0,3,10q�$	��392�� J� RenderTransformOrigin$ 0.425,0.588&� � ColumnDefinitions �� $��0* ���  � RowDefinitions �� $	��89* ��� $	��95* ��� $	��12* �.+��� -    ��Name$ outputListBox��$��	15,0,0,50q�$��1��$��2��$	��308��$��Bottom=�$
��Left=�$	��450���� -    ��Name$ listingGroupBox��$��Select Item To Dump��$��
477,1,11,0q�$��1��$��2��$	��355��$	��Top=�.���� -   	 ��Name$	 image��$��
118,-4,0,0q�$
��Left=�$	��121��$	��114��$	��Top=�	� -   
 	�Name$
 baseRadioButton��$��	Base Game��$
��Left=�$	��Top=�$��
491,30,0,0q�$ 0.585,3.766&� d� 	IsChecked$
 TrueY�$��1��$��15��$��78��	� -   $
 patchRadioButton��$��
Game Patch��$
��Left=�$	��Top=�$��
491,50,0,0q�$ 0.478,-1.024&�$��1��$��15��$��82��	� -   $
 acRadioButton��$��
Loaded DLC��$
��Left=�$	��Top=�$��
491,70,0,0q�$ 0.595,-1.289&�$��1��$��15��$��83��{�.  	� -   $
 allRadioButton��$	��All��$
��Left=�$	��Top=�$��
491,90,0,0q�$ 0.595,-1.289&�$��1��$��15��$��33���� -	    ��Name$ 	ipTextBox��$
��Left=�$��23�� �� TextWrapping$
 Wrap=�$��10.0.0.6��$	��Top=�$	��113��$��603,160,0,0q�$ 2.451,1.448&�$��1��$��2���� -
   $ portTextBox��$
��Left=�$��23��$
 Wrap=�$
��2121��$��Bottom=�$��43��$��603,0,0,159q�$��1��$��1���� -    ��Name$ loadingLabel��$�� ��$
��Left=�$	��Top=�$��	1,145,0,0q�$��26��$	��464��$��1��$��1������  �� EndPoint$ 0.5,1&� �� 
StartPoint$ 0.5,0&�.���  � Color$ 	#FF1B4706�� � Offset$ 0.118Q�� $ 	#FF009DFF��$ 1Q�� $ 	#FF070780��$ 0.114Q�� $ 	#FF06067D��$
 0.98Q�� $ 	#FF2B0101��$ 0.264Q�� $ 	#FF2B0101��$ 0.882Q��� -   $ keystoneLabel��$�� ��$
��Left=�$	��Top=�$��
491,30,0,0q�$��1��$��26��$	��232��$��1���� -   $ npLabel��$�� ��$
��Left=�$	��Top=�$��
491,45,0,0q�$��1��$��26��$	��232��$��1���� -   $ 	cusaLabel��$�� ��$
��Left=�$	��Top=�$��
491,60,0,0q�$��1��$��26��$	��230��$��1���� -   $ appVerLabel��$�� ��$
��Left=�$	��Top=�$��
491,76,0,0q�$��1��$��26��$	��232��$��1���� -   $ verLabel��$�� ��$
��Left=�$	��Top=�$��
491,91,0,0q�$��1��$��26��$	��230��$��1���� -    ��Name$ button1��$��Connect��$
��Left=�$��Bottom=�$��63��$��653,0,0,159q�$ 1.036,-9.35&�$��23��$��1��$��1���� -   $ button��$��Download��$
��Left=�$	��Top=�$��63��$��653,140,0,0q�$ 0.478,1.303&�$��23��$��1��$��1���� -   $ label��$��
PS4 Port: ��$
��Left=�$��Bottom=�$��493,0,0,160q�$ 0.995,0.381&�$��1��$��26��$��60��$��1���� -   $ ipLabel��$��PS4 IP Address: ��$��493,158,0,184q�$ 0.022,0.434&�$��1��$��2��$
��Left=�$	��110���� -   $ button2��$��	Check SFO��$��678,181,0,0q�$��2��$��1��$
��Left=�$��75��$��20��$	��Top=�$��1��$y�Hidden=��� -    ��Name$ checkBox��$��
Build FPKG��$
��Left=�$	��Top=�$��569,145,0,0q�$��1��$��15��$��77��$��1��{�.  �� -   $ cidLabel��$�� ��$
��Left=�$	��Top=�$��	1,164,0,0q�$��33��$	��465��$��2��$��1��$��1���� -   $ prcnt��$�� ��$
��Left=�$	��Top=�$��1,2,0,-4q�$	��465��$��2��$��1��$��26���� -   $ sendPayloadCheckBox��$��Send Payload��$
��Left=�$	��Top=�$��1��$��498,137,0,0q�$��15��$��91���� -    ��Name$ comboBox��$
��Left=�$	��Top=�$	��113��$��1��$��603,133,0,0q�$��25���� -   $ 	gameLabel��$�� ��$	��Top=�$��491,104,29,0q�$��1��$��26��$��1���� -   $ 	codeLabel��$�� ��$
��Left=�$	��Top=�$��1��$��481,180,0,0q�$��1��$��26��$	��194��$��2���� -   $ sfoGroupBox��$��SFO Information: ��$	��355��$	��Top=�$��1��$��830,1,-475,0q�$��2��.���� -   $ sfoTITLELabel��$��TITLE��$	��Top=�$��
10,34,11,0q��� -   $ APP_VERLabel��$��APP_VER��$	��Top=�$��1��$��845,73,-341,0q�$��26���� -    $ ATTRIBUTELabel��$��	ATTRIBUTE��$	��Top=�$��1��$��845,87,-341,0q�$��26���� -!   $ ATTRIBUTE2Label��$��
ATTRIBUTE2��$	��Top=�$��1��$��845,101,-341,0q�$��26���� -"   $ CATEGORYLabel��$��
CONTENT_ID��$	��Top=�$��1��$��845,116,-341,0q�$��26���� -#   $ PUBTOOLINFOcLabel��$��PUBTOOLINFO c_date:��$	��Top=�$��1��$��845,131,-341,0q�$��26���� -$   $ PUBTOOLINFOsdkLabel��$��PUBTOOLINFO sdk_ver:��$	��Top=�$��1��$��845,146,-341,0q�$��26���� -%   $ PUBTOOLINFOstLabel��$��PUBTOOLINFO st_type:��$	��Top=�$��1��$��845,161,-341,0q�$��2��$��26���� -&   $ PUBTOOLINFOl0Label��$��PUBTOOLINFO img0_l0_size:��$	��Top=�$��1��$��845,176,-341,0q�$��2��$��26���� -'   $ PUBTOOLINFOl1Label��$ ��PUBTOOLINFO img0_l1_size: ��$	��Top=�$��1��$��845,13,-341,0q�$��1��$��26���� -(   $ PUBTOOLINFOscLabel��$ ��PUBTOOLINFO img0_sc_ksize:��$	��Top=�$��1��$��845,28,-341,0q�$��1��$��26���� -)   $ PUBTOOLINFOpcLabel��$ ��PUBTOOLINFO img0_pc_ksize:��$	��Top=�$��1��$��845,43,-341,0q�$��1��$��26���� -*   $ VERSIONsLabel��$��VERSION:��$	��Top=�$��1��$��845,58,-341,0q�$��1��$��26���� -+   $ button3��$��Button��$
��Left=�$	��Top=�$��75��$��1��$��568,164,0,0q�$��1��$��20��$y�Hidden=�