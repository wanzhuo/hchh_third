

-- 客服信息
CREATE TABLE `shopserviceuserinfo` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `IsDel` tinyint(11) DEFAULT '0',
  `ShopId` int(32) NOT NULL,
  `AddTime` datetime NOT NULL,
  `openid` varchar(225) DEFAULT NULL,
  `nickname` varchar(100) DEFAULT NULL,
  `sex` varchar(25) DEFAULT NULL,
  `province` varchar(100) DEFAULT NULL,
  `city` varchar(100) DEFAULT NULL,
  `country` varchar(100) DEFAULT NULL,
  `headimgurl` varchar(300) DEFAULT NULL,
  `unionid` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=65 DEFAULT CHARSET=utf8;




