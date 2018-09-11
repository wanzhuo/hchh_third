CREATE TABLE `shop`.`ShopBrand` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `Flag` varchar(45) DEFAULT NULL,
  `Name` varchar(45) DEFAULT NULL,
  `Address` varchar(255) DEFAULT NULL,
  `Detail` varchar(255) DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  `AddIp` varchar(45) DEFAULT NULL,
  `AddUser` varchar(45) DEFAULT NULL,
  `Status` int(32) DEFAULT 0,
  PRIMARY KEY (`Id`)
);

CREATE TABLE `shop`.`Shop` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `Flag` varchar(45) DEFAULT NULL,
  `Name` varchar(45) DEFAULT NULL,
  `UsePerUser` varchar(45) DEFAULT NULL,
  `Address` varchar(255) DEFAULT NULL,
  `AddressEx` varchar(255) DEFAULT NULL,
  `Detail` varchar(255) DEFAULT NULL,
  `Tel` varchar(45) DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  `AddIp` varchar(45) DEFAULT NULL,
  `AddUser` varchar(45) DEFAULT NULL,
  `Status` int(32) DEFAULT 0,
  PRIMARY KEY (`Id`)
);