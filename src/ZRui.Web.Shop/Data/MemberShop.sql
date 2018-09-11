
-- 会员信息
CREATE TABLE `shopmember` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `IsDel` tinyint(11) DEFAULT '0',
  `Name` varchar(60) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_520_ci DEFAULT NULL,
  `Sex` varchar(10) DEFAULT NULL,
  `Phone` varchar(20) DEFAULT NULL,
  `Credits` int(32) DEFAULT '0',
  `Balance` int(32) DEFAULT '0',
  `ShopMemberLevelId` int(32) DEFAULT '0',
  `ShopId` int(32) NOT NULL,
  `MemberId` int(32) NOT NULL,
  `BirthDay` datetime NOT NULL,
  `AddTime` datetime NOT NULL,
  `AddIp` varchar(20) DEFAULT NULL,
  `PayPassword` varchar(40) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=45 DEFAULT CHARSET=utf8;





-- 商铺会员等级
CREATE TABLE `shopmemberlevel` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `IsDel` tinyint(11) DEFAULT '0',
  `ShopId` int(32) NOT NULL,
  `AddTime` datetime NOT NULL,
  `AddIp` varchar(20) DEFAULT NULL,
  `LevelName` varchar(100) DEFAULT NULL,
  `MemberLevel` varchar(100) DEFAULT NULL,
  `MaxIntegral` int(32) DEFAULT NULL,
  `MinIntegral` int(32) DEFAULT NULL,
  `Discount` double(16,2) DEFAULT NULL,
  `Sort` int(10) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=65 DEFAULT CHARSET=utf8;



-- 自定义金额充值设置
CREATE TABLE `shopcustomtopupset` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `IsDel` tinyint(11) DEFAULT '0',
  `ShopId` int(32) NOT NULL,
  `AddTime` datetime NOT NULL,
  `AddIp` varchar(20) DEFAULT NULL,
  `StartAmount` int(32) NOT NULL,
  `MeetAmount` int(32) NOT NULL,
  `Additional` double(16,2) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;


-- 会员设置
CREATE TABLE `shopmemberset` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `IsDel` tinyint(11) DEFAULT '0',
  `ShopId` int(32) NOT NULL,
  `AddTime` datetime NOT NULL,
  `AddIp` varchar(20) DEFAULT NULL,
  `IsSavaLevel` tinyint(1) NOT NULL,
  `IsTopUpDiscount` tinyint(1) NOT NULL,
  `IsConsumeIntegral` tinyint(1) NOT NULL,
  `IsTopUpIntegral` tinyint(1) NOT NULL,
  `ConsumeAmount` int(100) NOT NULL,
  `GetIntegral` int(32) NOT NULL,
  `IsShowCustomTopUpSet` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;



-- 固定金额充值设置
CREATE TABLE `shoptopupset` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `IsDel` tinyint(11) DEFAULT '0',
  `ShopId` int(32) NOT NULL,
  `AddTime` datetime NOT NULL,
  `AddIp` varchar(20) DEFAULT NULL,
  `FixationTopUpAmount` int(100) NOT NULL,
  `PresentedAmount` int(100) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=122 DEFAULT CHARSET=utf8;




-- 商铺会员充值记录
CREATE TABLE `shopmemberrecharge` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `IsDel` tinyint(1) DEFAULT '0',
  `ShopMemberId` int(32) NOT NULL,
  `Amount` int(32) NOT NULL,
  `PresentedAmount` int(32) DEFAULT '0',
  `TransactionTime` datetime NOT NULL,
  `Status` int(32) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=92 DEFAULT CHARSET=utf8;

-- 会员卡信息
DROP TABLE IF EXISTS `shopmembercardinfo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `shopmembercardinfo` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `IsDel` tinyint(11) DEFAULT '0',
  `ShopId` int(32) NOT NULL,
  `AddTime` datetime NOT NULL,
  `AddIp` varchar(20) DEFAULT NULL,
  `PrivilegeExplain` varchar(400) DEFAULT NULL,
  `ValidityBeginTime` datetime DEFAULT NULL,
  `ValidityEndTime` datetime DEFAULT NULL,
  `IsValidityLong` tinyint(1) NOT NULL,
  `UsedKnow` varchar(500) DEFAULT NULL,
  `CardCover` varchar(500) NOT NULL DEFAULT 'http://91huichihuihe.oss-cn-shenzhen.aliyuncs.com/DmHsyw_1536202883673.png',
  `ServePhone` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8;
-- 商铺会员消费记录
CREATE TABLE `ShopMemberConsume` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `IsDel` tinyint(1) DEFAULT 0,
  `ShopMemberId` int(32) NOT NULL,
  `Amount` int(32) NOT NULL,
  `TransactionTime` datetime NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;




-- 商铺会员退款记录
CREATE TABLE `shopmemberrufund` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `IsDel` tinyint(1) DEFAULT '0',
  `ShopOrderId` int(32) NOT NULL DEFAULT '0',
  `ShopMemberId` int(32) NOT NULL,
  `Amount` int(32) NOT NULL,
  `TransactionTime` datetime NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=93 DEFAULT CHARSET=utf8;


-- 商铺积分记录
CREATE TABLE `shopintegralrecharge` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `IsDel` tinyint(1) DEFAULT '0',
  `ShopId` int(32) NOT NULL,
  `AddTime` datetime NOT NULL,
  `AddIp` varchar(20) DEFAULT NULL,
  `SourceType` int(5) NOT NULL,
  `SourceRemark` varchar(100) DEFAULT NULL,
  `CodeStatut` int(5) NOT NULL,
  `MemberId` int(32) NOT NULL,
  `Count` int(32) NOT NULL,
  `SourceOrderId` int(32) DEFAULT NULL,
  `ShopMemberId` int(32) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=145 DEFAULT CHARSET=utf8;


ALTER TABLE shopbrandcommodity add UseMemberPrice TINYINT DEFAULT 0
ALTER TABLE ShopOrder add MemberDiscount INT DEFAULT 0
ALTER TABLE ShopOrder add ShopMemberConsumeId INT
ALTER TABLE ShopOrderItem add PrimePrice INT DEFAULT 0
ALTER TABLE ConglomerationActivity  add ConglomerationActivityStatut INT(11) DEFAULT 2


ALTER TABLE `hchh`.`shoporder` 
ADD COLUMN `PayWay` VARCHAR(45) NULL AFTER `takeDistributionType`;

ALTER TABLE `hchh`.`conglomerationorder` 
ADD COLUMN `PayWay` VARCHAR(45) NULL AFTER `FormId`;

