

/*打印机添加记录*/
create table Printer
(
 ID int primary key not null auto_increment,#编号
 ShopID int not null,#商铺编号
 SN varchar(32) not null,#打印机编号
 SKey varchar(32) not null,#打印机key
 AddTime datetime default now(),#添加时间
 IsEnable  tinyint default 1,#是否启用
 IsSuccess tinyint default 1, #是否添加成功
 IsDel tinyint default 0,
 PrinterType varchar(50) not null  default 'FEIE', #打印机的品牌
 Times int default 1 #打印联数,
 ModelID int  default 1 #模板编号
)
/*打印记录*/
create table PrintRecord
(
  ID int primary key not null auto_increment,#编号
  SN varchar(32) not null,#打印机编号
  OrderID varchar(50) not null,#订单编号 该编号由打印成功之后返回
  Title varchar(100) not null,#标题
  OrderList  varchar(2000) not null,#订单列表，存储的是JSon格式的数据
  TotalMoney float default 0,#总金额，由后台进行计算
  Address varchar(100),#送货地址
  OrderName varchar(50), #下单人
  Mobile varchar(20),#下单人电话
  OrderTime datetime  default now(),#下单时间
  QRAddress varchar(100),#二维码地址
  Remark varchar(500),#备注
   IsDel tinyint default 0
)
/*打印模板*/
create table PrintModel
(
 ID int  primary key not null auto_increment,#编号，
 Flag varchar(32) not null ,
 ModelContent varchar(1000) not null
)