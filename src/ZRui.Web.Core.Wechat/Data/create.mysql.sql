use wechatCore;

CREATE TABLE `CustomerSmsValiCodeTask` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `Phone` varchar(25) DEFAULT NULL,
  `IP` varchar(25) DEFAULT NULL,
  `Code` varchar(25) DEFAULT NULL,
  `TaskState` int(11) DEFAULT NULL,
  `TaskType` int(11) DEFAULT NULL,
  `TaskTime` datetime DEFAULT NULL,
  `TaskEndTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;



CREATE TABLE `CustomerPhone` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `OpenId` varchar(45) DEFAULT NULL,
  `Phone` varchar(45) DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  `Status` int(32) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;



CREATE TABLE `RequestMsgData` (
  `Id` bigint(64) NOT NULL AUTO_INCREMENT,
  `ToUserName` varchar(45) DEFAULT NULL,
  `FromUserName` varchar(45) DEFAULT NULL,
  `CreateTime` datetime DEFAULT NULL,
  `MsgType` int(11) DEFAULT NULL,
  `MsgId` bigint(64) DEFAULT NULL,
  `Content` varchar(2000) CHARACTER SET utf8 DEFAULT NULL,
  `Xml` varchar(2000) CHARACTER SET utf8 DEFAULT NULL,
  `Status` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;

CREATE TABLE `CustomerSession` (
  `Id` bigint(64) NOT NULL AUTO_INCREMENT,
  `OpenId` varchar(45) DEFAULT NULL,
  `Time` datetime DEFAULT NULL,
  `Worker` varchar(1000) DEFAULT NULL,
  `Status` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;

CREATE TABLE `CustomerMessage` (
  `Id` bigint(64) NOT NULL AUTO_INCREMENT,
  `FromUser` varchar(45) DEFAULT NULL,
  `Content` varchar(2000) CHARACTER SET utf8 DEFAULT NULL,CREATE TABLE `CustomerMessage` (
  `Id` bigint(64) NOT NULL AUTO_INCREMENT,
  `FromUser` varchar(45) CHARACTER SET utf8 DEFAULT NULL,
  `Content` varchar(2000) CHARACTER SET utf8 DEFAULT NULL,
  `Time` datetime DEFAULT NULL,
  `ToUser` varchar(45) DEFAULT NULL,
  `ChatFlag` varchar(45) DEFAULT NULL,
  `MemberIsRead` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=117 DEFAULT CHARSET=latin1;

CREATE TABLE `CustomerPhone` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `OpenId` varchar(45) DEFAULT NULL,
  `Phone` varchar(45) DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  `Status` int(32) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=latin1;

CREATE TABLE `CustomerSession` (
  `Id` bigint(64) NOT NULL AUTO_INCREMENT,
  `OpenId` varchar(45) DEFAULT NULL,
  `Time` datetime DEFAULT NULL,
  `Worker` varchar(1000) DEFAULT NULL,
  `Status` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=latin1;

CREATE TABLE `CustomerSmsValiCodeTask` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `Phone` varchar(25) DEFAULT NULL,
  `IP` varchar(25) DEFAULT NULL,
  `Code` varchar(25) DEFAULT NULL,
  `TaskState` int(11) DEFAULT NULL,
  `TaskType` int(11) DEFAULT NULL,
  `TaskTime` datetime DEFAULT NULL,
  `TaskEndTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=latin1;

CREATE TABLE `MemberWechat` (
  `Id` bigint(64) NOT NULL AUTO_INCREMENT,
  `MemberId` int(11) DEFAULT NULL,
  `OpenId` varchar(100) CHARACTER SET utf8 DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=27 DEFAULT CHARSET=latin1;

CREATE TABLE `MemberWeChatBindTask` (
  `Id` bigint(64) NOT NULL AUTO_INCREMENT,
  `Code` varchar(45) DEFAULT NULL,
  `MemberId` int(11) DEFAULT NULL,
  `OpenId` varchar(45) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  `AddIp` varchar(45) DEFAULT NULL,
  `Status` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=latin1;

CREATE TABLE `MemberWeChatLoginTask` (
  `Id` bigint(64) NOT NULL AUTO_INCREMENT,
  `ClientId` varchar(45) DEFAULT NULL,
  `Code` varchar(45) DEFAULT NULL,
  `OpenId` varchar(45) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  `AddIp` varchar(45) DEFAULT NULL,
  `Status` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=latin1;

CREATE TABLE `RequestMsgData` (
  `Id` bigint(64) NOT NULL AUTO_INCREMENT,
  `ToUserName` varchar(45) DEFAULT NULL,
  `FromUserName` varchar(45) DEFAULT NULL,
  `CreateTime` datetime DEFAULT NULL,
  `MsgType` int(11) DEFAULT NULL,
  `MsgId` bigint(64) DEFAULT NULL,
  `Content` varchar(2000) CHARACTER SET utf8 DEFAULT NULL,
  `Xml` varchar(2000) CHARACTER SET utf8 DEFAULT NULL,
  `Status` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=41 DEFAULT CHARSET=latin1;

CREATE TABLE `WechatQRScene` (
  `Id` bigint(64) NOT NULL AUTO_INCREMENT,
  `SceneId` int(11) DEFAULT NULL,
  `QrCodeTicket` varchar(100) CHARACTER SET utf8 DEFAULT NULL,
  `Category` varchar(45) DEFAULT NULL,
  `Status` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=69 DEFAULT CHARSET=latin1;

  `Time` datetime DEFAULT NULL,
  `ToUser` varchar(45) DEFAULT NULL,
  `ChatFlag` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=latin1;

alter table `wechatCore`.`CustomerMessage`
Add column `MemberIsRead` int(11) DEFAULT NULL AFTER `ChatFlag`;

CREATE TABLE `WechatQRScene` (
  `Id` bigint(64) NOT NULL AUTO_INCREMENT,
  `SceneId` int(11) DEFAULT NULL,
  `QrCodeTicket` varchar(100) CHARACTER SET utf8 DEFAULT NULL,
  `Category` varchar(45) DEFAULT NULL,
  `Status` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=latin1;

CREATE TABLE `MemberWechat` (
  `Id` bigint(64) NOT NULL AUTO_INCREMENT,
  `MemberId` int(11) DEFAULT NULL,
  `OpenId` varchar(100) CHARACTER SET utf8 DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=latin1;

CREATE TABLE `MemberWeChatBindTask` (
  `Id` bigint(64) NOT NULL AUTO_INCREMENT,
  `Code` varchar(45) DEFAULT NULL,
  `MemberId` int(11) DEFAULT NULL,
  `OpenId` varchar(45) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  `AddIp` varchar(45) DEFAULT NULL,
  `Status` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;

CREATE TABLE `MemberWeChatLoginTask` (
  `Id` bigint(64) NOT NULL AUTO_INCREMENT,
  `ClientId` varchar(45) DEFAULT NULL,
  `Code` varchar(45) DEFAULT NULL,
  `OpenId` varchar(45) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  `AddIp` varchar(45) DEFAULT NULL,
  `Status` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;