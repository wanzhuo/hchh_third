-- MySQL dump 10.13  Distrib 5.7.17, for Win64 (x86_64)
--
-- Host: 119.147.144.104    Database: shop
-- ------------------------------------------------------
-- Server version	5.7.17

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
D:\project\HuiChiHuiHe\hchh\src\ZRui.Web.Shop\Data\ConglomerationActivityType.cs
--
-- Table structure for table `CommercialDistrict`
--

DROP TABLE IF EXISTS `CommercialDistrict`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `CommercialDistrict` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `Flag` varchar(45) DEFAULT NULL,
  `Name` varchar(45) DEFAULT NULL,
  `Address` varchar(255) DEFAULT NULL,
  `Detail` varchar(255) DEFAULT NULL,
  `Latitude` double DEFAULT NULL,
  `Longitude` double DEFAULT NULL,
  `GeoHash` varchar(128) DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  `AddIp` varchar(45) DEFAULT NULL,
  `AddUser` varchar(45) DEFAULT NULL,
  `Status` int(32) DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CommercialDistrict`
--

LOCK TABLES `CommercialDistrict` WRITE;
/*!40000 ALTER TABLE `CommercialDistrict` DISABLE KEYS */;
INSERT INTO `CommercialDistrict` VALUES (1,'24b61aa3-2bfe-4915-89bc-df481fb952f9','洪福路口','莞太路',NULL,1,2,'s01mtw037ms0',0,'2018-03-06 09:20:14','::1','member1',0);
/*!40000 ALTER TABLE `CommercialDistrict` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CommercialDistrictShop`
--

DROP TABLE IF EXISTS `CommercialDistrictShop`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `CommercialDistrictShop` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `CommercialDistrictId` int(32) DEFAULT NULL,
  `ShopId` int(32) DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  `AddIp` varchar(45) DEFAULT NULL,
  `AddUser` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CommercialDistrictShop`
--

LOCK TABLES `CommercialDistrictShop` WRITE;
/*!40000 ALTER TABLE `CommercialDistrictShop` DISABLE KEYS */;
INSERT INTO `CommercialDistrictShop` VALUES (1,1,6,0,'2018-03-06 09:22:24','::1','member1');
/*!40000 ALTER TABLE `CommercialDistrictShop` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `SettingBase`
--

DROP TABLE IF EXISTS `SettingBase`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `SettingBase` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `Flag` varchar(45) DEFAULT NULL,
  `GroupFlag` varchar(45) DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  `Value` varchar(255) DEFAULT NULL,
  `Detail` varchar(255) DEFAULT NULL,
  `SettingType` int(32) DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `SettingBase`
--

LOCK TABLES `SettingBase` WRITE;
/*!40000 ALTER TABLE `SettingBase` DISABLE KEYS */;
INSERT INTO `SettingBase` VALUES (3,'ShopCallingQueue_OpenStatus_5','',0,'True',NULL,0),(4,'ShopCallingQueue_OpenStatus_1','',0,'True',NULL,0);
/*!40000 ALTER TABLE `SettingBase` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `Shop`
--

DROP TABLE IF EXISTS `Shop`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Shop` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `ShopBrandId` int(32) DEFAULT NULL,
  `Flag` varchar(45) DEFAULT NULL,
  `Name` varchar(45) DEFAULT NULL,
  `UsePerUser` varchar(45) DEFAULT NULL,
  `Address` varchar(255) DEFAULT NULL,
  `AddressGuide` varchar(255) DEFAULT NULL,
  `Latitude` double DEFAULT NULL,
  `Longitude` double DEFAULT NULL,
  `GeoHash` varchar(128) DEFAULT NULL,
  `Tel` varchar(45) DEFAULT NULL,
  `OpenTime` varchar(45) DEFAULT NULL,
  `ScoreValue` int(32) DEFAULT '0',
  `Detail` varchar(255) DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  `AddIp` varchar(45) DEFAULT NULL,
  `AddUser` varchar(45) DEFAULT NULL,
  `Status` int(32) DEFAULT '0',
`IsShowApplets` int(32) DEFAULT '0',
`Banners` varchar(5000) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `Geohash` (`GeoHash`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `Shop`
--

LOCK TABLES `Shop` WRITE;
/*!40000 ALTER TABLE `Shop` DISABLE KEYS */;
INSERT INTO `Shop` VALUES (1,1,'68b0eb87-6aa4-408a-b062-b0a3319ef17b','添添 天安店','80元/人','东莞市南城区天安路1号天安数码城','这是好地方',22.995726372,113.7106833747,'ws0fgd1xffnf','10086','早',10,'说明',0,'2018-01-29 21:38:33','::1','member1',0),(2,4,'f6b18669-d853-4aa5-819f-bb0644687d56','老山城重庆小面',NULL,'--',NULL,22.9971177108,113.7120723526,'ws0fgd6xgm6w',NULL,NULL,0,NULL,0,'2018-01-29 22:40:33','::1','member1',0),(3,3,'1090153d-cd94-48de-a332-00a8a50f6a15','秦关面道（天安店）',NULL,'--',NULL,22.9966633877,113.7124907772,'ws0fgd6vm0f8',NULL,NULL,0,NULL,0,'2018-01-29 22:42:34','::1','member1',0),(4,4,'f407e154-ad81-4a85-8158-83db96730900','小四川南城分店',NULL,'---',NULL,23.0161807943,113.7440907053,'ws0fumcyvrj6',NULL,NULL,0,NULL,0,'2018-01-30 11:19:44','::1','member1',0),(5,4,'74cee5a6-dd0d-4cc8-b60b-f603384a2992','黄振龙凉茶',NULL,'---',NULL,23.0221755389,113.7504599987,'ws0furn9cudd',NULL,NULL,0,NULL,0,'2018-01-30 11:24:38','::1','member1',0),(6,3,'059d3b13-212c-4e7c-876a-4fa93a2c5f40','秦关面道（洪福店）',NULL,NULL,NULL,NULL,NULL,'',NULL,NULL,0,NULL,0,'2018-02-06 22:08:53','::1','member1',0),(7,3,'befd6f9f-156e-46ff-8546-01f865469cf0','秦关面道汇一城店',NULL,'东莞市南城区第一国际汇一城',NULL,23.013725,113.761212,'ws0futw2sm7b',NULL,NULL,0,NULL,0,'2018-03-01 23:53:54','120.86.97.39','member1',0);
/*!40000 ALTER TABLE `Shop` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ShopBooking`
--

DROP TABLE IF EXISTS `ShopBooking`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ShopBooking` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `MemberId` int(32) DEFAULT '0',
  `ShopId` int(32) DEFAULT NULL,
  `Flag` varchar(45) DEFAULT NULL,
  `Users` varchar(128) DEFAULT NULL,
  `Nickname` varchar(45) DEFAULT NULL,
  `Phone` varchar(45) DEFAULT NULL,
  `Remark` varchar(255) DEFAULT NULL,
  `DinnerTime` datetime DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  `IsUsed` int(11) DEFAULT '0',
  `RefuseReason` varchar(255) DEFAULT NULL,
  `Status` int(32) DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ShopBooking`
--

LOCK TABLES `ShopBooking` WRITE;
/*!40000 ALTER TABLE `ShopBooking` DISABLE KEYS */;
INSERT INTO `ShopBooking` VALUES (7,14,5,NULL,'20个人','张三','13800138000','不加辣','2018-03-02 20:08:24',0,'2018-03-02 17:38:06',0,NULL,0);
/*!40000 ALTER TABLE `ShopBooking` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ShopBrand`
--

DROP TABLE IF EXISTS `ShopBrand`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ShopBrand` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `Flag` varchar(45) DEFAULT NULL,
  `Name` varchar(45) DEFAULT NULL,
  `Address` varchar(255) DEFAULT NULL,
  `Detail` varchar(255) DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  `AddIp` varchar(45) DEFAULT NULL,
  `AddUser` varchar(45) DEFAULT NULL,
  `Status` int(32) DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ShopBrand`
--

LOCK TABLES `ShopBrand` WRITE;
/*!40000 ALTER TABLE `ShopBrand` DISABLE KEYS */;
INSERT INTO `ShopBrand` VALUES (1,'861eb8dc-3d06-4a02-899d-ab8906f0b9d7','添添聚源味','东莞市南城区总部',NULL,0,'2018-01-29 21:18:05','::1','member1',0),(2,'37f0a645-5e15-41a9-86c4-ba5187ed0233','测试删除1','要删除的1',NULL,1,'2018-01-29 21:18:46','::1','member1',-1),(3,'85728b92-0454-4167-b55f-321cab760a1f','秦关面道',NULL,NULL,0,'2018-02-06 22:01:00','::1','member1',0),(4,'c7d4761e-83e2-4649-a0f2-a32a2a6b907c','自家品牌',NULL,NULL,0,'2018-02-06 22:03:12','::1','member1',0);
/*!40000 ALTER TABLE `ShopBrand` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ShopBrandCommodity`
--

DROP TABLE IF EXISTS `ShopBrandCommodity`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ShopBrandCommodity` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `CategoryId` int(32) DEFAULT NULL,
  `ShopBrandId` int(32) DEFAULT NULL,
  `Flag` varchar(45) DEFAULT NULL,
  `Name` varchar(45) DEFAULT NULL,
  `Price` decimal(10,3) DEFAULT '0.000',
  `Unit` varchar(45) DEFAULT NULL,
  `Summary` varchar(255) DEFAULT NULL,
  `Detail` varchar(4000) DEFAULT NULL,
  `SalesForMonth` int(32) DEFAULT '0',
  `Upvote` int(32) DEFAULT '0',
  `IsRecommand` int(11) DEFAULT NULL,
  `Cover` varchar(255) DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  `AddIp` varchar(45) DEFAULT NULL,
  `AddUser` varchar(45) DEFAULT NULL,
  `Status` int(32) DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ShopBrandCommodity`
--

LOCK TABLES `ShopBrandCommodity` WRITE;
/*!40000 ALTER TABLE `ShopBrandCommodity` DISABLE KEYS */;
INSERT INTO `ShopBrandCommodity` VALUES (1,4,1,'0a50edb2-e2ff-4808-9bbb-6a5e95e3327d','萝卜牛杂饭',19.000,'份','1','2',10,20,1,'123',0,'2018-02-02 22:19:16','::1','member1',0),(2,4,1,'50c56d25-e56c-406a-b5ad-f21fe17ab25f','烧鸭饭',0.000,NULL,NULL,NULL,0,0,0,NULL,0,'2018-02-07 17:41:34','::1','member1',0),(3,4,1,'3dcb264f-2e8b-445b-8011-fa769672af6d','包子',0.000,NULL,NULL,NULL,0,0,0,NULL,0,'2018-02-07 17:43:03','::1','member1',0),(4,3,1,'a3ee3cd3-cfd0-4459-8c82-6733baacd7e5','西米露',10.000,NULL,'我是简介',NULL,0,0,0,NULL,0,'2018-03-01 15:35:54','14.120.115.70','member1',0);
/*!40000 ALTER TABLE `ShopBrandCommodity` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ShopBrandCommodityCategory`
--

DROP TABLE IF EXISTS `ShopBrandCommodityCategory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ShopBrandCommodityCategory` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `ShopBrandId` int(32) DEFAULT NULL,
  `Flag` varchar(45) DEFAULT NULL,
  `Name` varchar(45) DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  `PId` int(32) DEFAULT NULL,
  `OrderWeight` float DEFAULT NULL,
  `Ico` varchar(255) DEFAULT NULL,
  `Tags` varchar(255) DEFAULT NULL,
  `Keywords` varchar(255) DEFAULT NULL,
  `Description` varchar(255) DEFAULT NULL,
  `Detail` varchar(2550) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ShopBrandCommodityCategory`
--

LOCK TABLES `ShopBrandCommodityCategory` WRITE;
/*!40000 ALTER TABLE `ShopBrandCommodityCategory` DISABLE KEYS */;
INSERT INTO `ShopBrandCommodityCategory` VALUES (3,1,'a1f0ee0e-afd5-4bd8-b304-81849dd55afa','甜点',0,NULL,0,NULL,NULL,NULL,NULL,NULL),(4,1,'1c781c09-ec1e-4997-8ce4-997b7962d223','主食',0,NULL,0,NULL,NULL,NULL,NULL,NULL),(10,1,'81bf555d-88a2-4ffb-8f57-db53aa51d9eb','面食',1,4,0,NULL,NULL,NULL,NULL,NULL),(12,1,'9aec10ee-4604-4a84-9234-19d6796a4ad4','新建类别',1,4,0,NULL,NULL,NULL,NULL,NULL),(13,1,'7ec0b9a1-5261-44ac-8a89-740c8e73d43e','新建类别',1,3,0,NULL,NULL,NULL,NULL,NULL);
/*!40000 ALTER TABLE `ShopBrandCommodityCategory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ShopBrandCommodityParameter`
--

DROP TABLE IF EXISTS `ShopBrandCommodityParameter`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ShopBrandCommodityParameter` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `ShopBrandId` int(32) DEFAULT NULL,
  `Flag` varchar(45) DEFAULT NULL,
  `Name` varchar(45) DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  `AddIp` varchar(45) DEFAULT NULL,
  `AddUser` varchar(45) DEFAULT NULL,
  `Status` int(32) DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ShopBrandCommodityParameter`
--

LOCK TABLES `ShopBrandCommodityParameter` WRITE;
/*!40000 ALTER TABLE `ShopBrandCommodityParameter` DISABLE KEYS */;
INSERT INTO `ShopBrandCommodityParameter` VALUES (1,1,'f5e6692b-46d4-4a99-ae5a-65c515d55014','颜色',0,'2018-02-02 22:56:26','::1','member1',0),(2,1,'fcf6b4a8-6392-46a2-bdd0-be1b99435f4e','尺寸',0,'2018-02-04 17:59:53','::1','member1',0),(3,1,'0783127f-4821-45a1-82af-3ed567cafb8f','通用',0,'2018-03-01 16:58:26','119.147.144.166','member1',0);
/*!40000 ALTER TABLE `ShopBrandCommodityParameter` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ShopBrandCommodityParameterValue`
--

DROP TABLE IF EXISTS `ShopBrandCommodityParameterValue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ShopBrandCommodityParameterValue` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `ParameterId` int(32) DEFAULT NULL,
  `Value` varchar(45) DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  `AddIp` varchar(45) DEFAULT NULL,
  `AddUser` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ShopBrandCommodityParameterValue`
--

LOCK TABLES `ShopBrandCommodityParameterValue` WRITE;
/*!40000 ALTER TABLE `ShopBrandCommodityParameterValue` DISABLE KEYS */;
INSERT INTO `ShopBrandCommodityParameterValue` VALUES (2,1,'蓝色',0,'2018-02-04 17:33:52','::1','member1'),(3,1,'红色',0,'2018-02-04 17:58:01','::1','member1'),(4,1,'大红色',1,'2018-02-04 17:58:04','::1','member1'),(5,1,'绿色',0,'2018-02-04 17:59:21','::1','member1'),(6,2,'31',0,'2018-02-04 18:00:03','::1','member1'),(7,2,'32',0,'2018-02-04 18:00:07','::1','member1'),(8,2,'33',0,'2018-02-04 18:00:44','::1','member1'),(9,3,'通用',0,'2018-03-01 16:58:39','119.147.144.166','member1');
/*!40000 ALTER TABLE `ShopBrandCommodityParameterValue` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ShopBrandCommoditySku`
--

DROP TABLE IF EXISTS `ShopBrandCommoditySku`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ShopBrandCommoditySku` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `CommodityId` int(32) DEFAULT NULL,
  `Flag` varchar(45) DEFAULT NULL,
  `Summary` varchar(128) DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=73 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ShopBrandCommoditySku`
--

LOCK TABLES `ShopBrandCommoditySku` WRITE;
/*!40000 ALTER TABLE `ShopBrandCommoditySku` DISABLE KEYS */;
INSERT INTO `ShopBrandCommoditySku` VALUES (2,1,'1_6',NULL,1),(3,1,'1_7',NULL,1),(4,1,'1_8',NULL,1),(5,1,'1_2',NULL,1),(6,1,'1_3',NULL,1),(7,1,'1_4',NULL,1),(8,1,'1_5',NULL,1),(9,1,'1_6',NULL,1),(10,1,'1_7',NULL,1),(11,1,'1_8',NULL,1),(12,1,'1_8_3',NULL,1),(13,1,'1_8_2',NULL,1),(14,1,'1_7_5',NULL,1),(15,1,'1_7_4',NULL,1),(16,1,'1_7_3',NULL,1),(17,1,'1_7_2',NULL,1),(18,1,'1_6_4',NULL,1),(19,1,'1_8_4',NULL,1),(20,1,'1_6_3',NULL,1),(21,1,'1_6_2',NULL,1),(22,1,'1_6_5',NULL,1),(23,1,'1_8_5',NULL,1),(24,1,'1_6',NULL,1),(25,1,'1_7',NULL,1),(26,1,'1_8',NULL,1),(27,1,'1_8_3',NULL,1),(28,1,'1_8_2',NULL,1),(29,1,'1_7_5',NULL,1),(30,1,'1_7_4',NULL,1),(31,1,'1_7_3',NULL,1),(32,1,'1_7_2',NULL,1),(33,1,'1_6_4',NULL,1),(34,1,'1_8_4',NULL,1),(35,1,'1_6_3',NULL,1),(36,1,'1_6_2',NULL,1),(37,1,'1_6_5',NULL,1),(38,1,'1_8_5',NULL,1),(39,1,'1_3',NULL,1),(40,1,'1_2',NULL,1),(41,1,'1_4',NULL,1),(42,1,'1_5',NULL,1),(43,1,'1_5_6',NULL,1),(44,1,'1_4_8',NULL,1),(45,1,'1_4_7',NULL,1),(46,1,'1_4_6',NULL,1),(47,1,'1_3_8',NULL,1),(48,1,'1_3_7',NULL,1),(49,1,'1_3_6',NULL,1),(50,1,'1_2_7',NULL,1),(51,1,'1_5_7',NULL,1),(52,1,'1_2_6',NULL,1),(53,1,'1_2_8',NULL,1),(54,1,'1_5_8',NULL,1),(55,1,'1_3',NULL,1),(56,1,'1_2',NULL,1),(57,1,'1_4',NULL,1),(58,1,'1_5',NULL,1),(59,1,'1_5_6',NULL,0),(60,1,'1_4_8',NULL,0),(61,1,'1_4_7',NULL,0),(62,1,'1_4_6',NULL,0),(63,1,'1_3_8',NULL,0),(64,1,'1_3_7',NULL,0),(65,1,'1_3_6',NULL,0),(66,1,'1_2_7',NULL,0),(67,1,'1_5_7',NULL,0),(68,1,'1_2_6',NULL,0),(69,1,'1_2_8',NULL,0),(70,1,'1_5_8',NULL,0),(71,4,'4_9',NULL,0),(72,3,'3_9',NULL,0);
/*!40000 ALTER TABLE `ShopBrandCommoditySku` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ShopBrandCommoditySkuItem`
--

DROP TABLE IF EXISTS `ShopBrandCommoditySkuItem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ShopBrandCommoditySkuItem` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `SkuId` int(32) DEFAULT NULL,
  `ParameterId` int(32) DEFAULT NULL,
  `ParameterValueId` int(32) DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=121 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ShopBrandCommoditySkuItem`
--

LOCK TABLES `ShopBrandCommoditySkuItem` WRITE;
/*!40000 ALTER TABLE `ShopBrandCommoditySkuItem` DISABLE KEYS */;
INSERT INTO `ShopBrandCommoditySkuItem` VALUES (2,2,2,6,1),(3,3,2,7,1),(4,4,2,8,1),(5,5,1,2,1),(6,6,1,3,1),(7,7,1,4,1),(8,8,1,5,1),(9,9,2,6,1),(10,10,2,7,1),(11,11,2,8,1),(12,21,2,6,1),(13,19,1,4,1),(14,19,2,8,1),(15,12,1,3,1),(16,12,2,8,1),(17,13,1,2,1),(18,13,2,8,1),(19,14,1,5,1),(20,14,2,7,1),(21,15,1,4,1),(22,15,2,7,1),(23,16,1,3,1),(24,16,2,7,1),(25,17,1,2,1),(26,17,2,7,1),(27,22,1,5,1),(28,22,2,6,1),(29,18,1,4,1),(30,18,2,6,1),(31,20,1,3,1),(32,20,2,6,1),(33,21,1,2,1),(34,23,2,8,1),(35,23,1,5,1),(36,24,2,6,1),(37,25,2,7,1),(38,26,2,8,1),(39,36,2,6,1),(40,34,1,4,1),(41,34,2,8,1),(42,27,1,3,1),(43,27,2,8,1),(44,28,1,2,1),(45,28,2,8,1),(46,29,1,5,1),(47,29,2,7,1),(48,30,1,4,1),(49,30,2,7,1),(50,31,1,3,1),(51,31,2,7,1),(52,32,1,2,1),(53,32,2,7,1),(54,37,1,5,1),(55,37,2,6,1),(56,33,1,4,1),(57,33,2,6,1),(58,35,1,3,1),(59,35,2,6,1),(60,36,1,2,1),(61,38,2,8,1),(62,38,1,5,1),(63,40,1,2,1),(64,39,1,3,1),(65,41,1,4,1),(66,42,1,5,1),(67,52,1,2,1),(68,51,2,7,1),(69,51,1,5,1),(70,43,2,6,1),(71,43,1,5,1),(72,44,2,8,1),(73,44,1,4,1),(74,45,2,7,1),(75,45,1,4,1),(76,46,2,6,1),(77,46,1,4,1),(78,47,2,8,1),(79,47,1,3,1),(80,48,2,7,1),(81,48,1,3,1),(82,49,2,6,1),(83,49,1,3,1),(84,53,2,8,1),(85,53,1,2,1),(86,50,2,7,1),(87,50,1,2,1),(88,52,2,6,1),(89,54,1,5,1),(90,54,2,8,1),(91,56,1,2,1),(92,55,1,3,1),(93,57,1,4,1),(94,58,1,5,1),(95,68,1,2,0),(96,67,2,7,0),(97,67,1,5,0),(98,59,2,6,0),(99,59,1,5,0),(100,60,2,8,0),(101,60,1,4,0),(102,61,2,7,0),(103,61,1,4,0),(104,62,2,6,0),(105,62,1,4,0),(106,63,2,8,0),(107,63,1,3,0),(108,64,2,7,0),(109,64,1,3,0),(110,65,2,6,0),(111,65,1,3,0),(112,69,2,8,0),(113,69,1,2,0),(114,66,2,7,0),(115,66,1,2,0),(116,68,2,6,0),(117,70,1,5,0),(118,70,2,8,0),(119,71,3,9,0),(120,72,3,9,0);
/*!40000 ALTER TABLE `ShopBrandCommoditySkuItem` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ShopCallingQueue`
--

DROP TABLE IF EXISTS `ShopCallingQueue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ShopCallingQueue` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `MemberId` int(32) DEFAULT '0',
  `ShopId` int(32) DEFAULT NULL,
  `ProductId` int(32) DEFAULT NULL,
  `Flag` varchar(45) DEFAULT NULL,
  `Title` varchar(45) DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  `Remark` varchar(255) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  `CanShareTable` int(11) DEFAULT NULL,
  `QueueNumber` int(32) DEFAULT '0',
  `QueueIndex` int(32) DEFAULT '0',
  `IsUsed` int(11) DEFAULT '0',
  `RefuseReason` varchar(255) DEFAULT NULL,
  `Status` int(32) DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ShopCallingQueue`
--

LOCK TABLES `ShopCallingQueue` WRITE;
/*!40000 ALTER TABLE `ShopCallingQueue` DISABLE KEYS */;
INSERT INTO `ShopCallingQueue` VALUES (3,9,5,NULL,NULL,'20个人',0,NULL,'2018-02-22 11:57:56',0,1,10,1,'123',1),(4,9,5,4,NULL,'3-4人(标准桌)',0,NULL,'2018-02-22 22:24:25',1,2,8,1,NULL,1),(5,9,1,7,NULL,'3-4人桌',0,NULL,'2018-03-02 09:00:17',1,1,1,0,NULL,1),(6,10,1,6,NULL,'1-2人桌',0,NULL,'2018-03-02 09:52:24',1,2,2,0,NULL,1),(7,11,1,7,NULL,'3-4人桌',0,NULL,'2018-03-02 09:52:50',1,3,3,0,NULL,1),(8,12,5,4,NULL,'3-4人(标准桌)',0,NULL,'2018-03-02 09:53:14',1,4,4,0,NULL,1),(9,13,1,6,NULL,'1-2人桌',0,NULL,'2018-03-02 13:48:13',1,5,5,0,NULL,1);
/*!40000 ALTER TABLE `ShopCallingQueue` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ShopCallingQueueProduct`
--

DROP TABLE IF EXISTS `ShopCallingQueueProduct`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ShopCallingQueueProduct` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `ShopId` int(32) DEFAULT NULL,
  `Flag` varchar(45) DEFAULT NULL,
  `Title` varchar(45) DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  `Detail` varchar(255) DEFAULT NULL,
  `Status` int(32) DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ShopCallingQueueProduct`
--

LOCK TABLES `ShopCallingQueueProduct` WRITE;
/*!40000 ALTER TABLE `ShopCallingQueueProduct` DISABLE KEYS */;
INSERT INTO `ShopCallingQueueProduct` VALUES (3,5,NULL,'1-2人(小桌)',0,'123',0),(4,5,NULL,'3-4人(标准桌)',0,'321',0),(5,5,NULL,'测试删除',1,NULL,0),(6,1,NULL,'1-2人桌',0,NULL,0),(7,1,NULL,'3-4人桌',0,NULL,0),(8,1,NULL,'7-8人桌',0,NULL,0),(9,1,NULL,'10-12人桌',0,NULL,0),(10,1,NULL,'包房',0,NULL,0);
/*!40000 ALTER TABLE `ShopCallingQueueProduct` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ShopCommodityStock`
--

DROP TABLE IF EXISTS `ShopCommodityStock`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ShopCommodityStock` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `ShopId` int(32) DEFAULT NULL,
  `SkuId` int(32) DEFAULT NULL,
  `Stock` int(32) DEFAULT '0',
  `CostPrice` decimal(10,3) DEFAULT '0.000',
  `SalePrice` decimal(10,3) DEFAULT '0.000',
  `MarketPrice` decimal(10,3) DEFAULT '0.000',
  `IsDel` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ShopCommodityStock`
--

LOCK TABLES `ShopCommodityStock` WRITE;
/*!40000 ALTER TABLE `ShopCommodityStock` DISABLE KEYS */;
INSERT INTO `ShopCommodityStock` VALUES (5,1,58,0,0.000,0.000,0.000,1),(6,1,56,1,2.000,3.000,0.000,1),(7,1,58,0,0.000,0.000,0.000,1),(8,1,57,0,0.000,0.000,0.000,1),(9,1,70,7,0.000,15.000,3.000,0),(10,1,68,3,0.000,10.000,2.000,0),(11,1,66,3,0.000,5.000,1.000,0),(12,1,71,10,1000.000,10.000,0.000,0),(13,1,72,10,20.000,15.000,0.000,0);
/*!40000 ALTER TABLE `ShopCommodityStock` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ShopOrder`
--

-- ----------------------------
-- Table structure for shoporder
-- ----------------------------
DROP TABLE IF EXISTS `shoporder`;
CREATE TABLE `shoporder` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `Flag` varchar(64) DEFAULT NULL,
  `ShopId` int(32) DEFAULT NULL,
  `MemberId` int(32) DEFAULT NULL,
  `ShopPartId` int(32) DEFAULT NULL,
  `Phone` varchar(45) DEFAULT NULL,
  `Amount` int(32) DEFAULT NULL,
  `Remark` varchar(128) DEFAULT NULL,
  `PayTime` datetime DEFAULT NULL,
  `MoneyOffRuleId` int(32) DEFAULT NULL,
  `Payment` int(32) DEFAULT NULL,
  `FinishTime` datetime DEFAULT NULL,
  `IsTakeOut` int(11) DEFAULT '0',
  `IsDel` int(11) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  `AddUser` varchar(45) DEFAULT NULL,
  `AddIp` varchar(45) DEFAULT NULL,
  `Status` int(11) DEFAULT NULL,
  `IsPrint` int(11) DEFAULT '0',
  `IsSend` int(11) DEFAULT '0',
  `memberAddressId` int(32) DEFAULT NULL,
  `otherFeeId` int(32) DEFAULT NULL,
  `OrderNumber` varchar(45) DEFAULT NULL,
  `ShopOrderSelfHelpId` int(32) DEFAULT NULL,
  `MemberDiscount` int(11) DEFAULT '0',
  `ShopMemberConsumeId` int(11) DEFAULT NULL,
  `takeDistributionType` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2853 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ShopOrder`
--

LOCK TABLES `ShopOrder` WRITE;
/*!40000 ALTER TABLE `ShopOrder` DISABLE KEYS */;
INSERT INTO `ShopOrder` VALUES (5,'8fbafb9b-bb44-4d22-bf25-cbac8327bbe1',1,14,NULL,35,NULL,NULL,NULL,0,'2018-03-06 10:11:51','member14','::1',0);
/*!40000 ALTER TABLE `ShopOrder` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ShopOrderItem`
--

DROP TABLE IF EXISTS `ShopOrderItem`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ShopOrderItem` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `ShopOrderId` int(32) DEFAULT NULL,
  `CommodityStockId` int(32) DEFAULT NULL,
  `CommodityName` varchar(128) DEFAULT NULL,
  `SkuSummary` varchar(128) DEFAULT NULL,
  `Count` int(32) DEFAULT '0',
  `CostPrice` decimal(10,3) DEFAULT '0.000',
  `SalePrice` decimal(10,3) DEFAULT '0.000',
  `MarketPrice` decimal(10,3) DEFAULT '0.000',
  `IsDel` int(11) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  `AddUser` varchar(45) DEFAULT NULL,
  `AddIp` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ShopOrderItem`
--

LOCK TABLES `ShopOrderItem` WRITE;
/*!40000 ALTER TABLE `ShopOrderItem` DISABLE KEYS */;
INSERT INTO `ShopOrderItem` VALUES (5,5,9,'萝卜牛杂饭',NULL,1,0.000,15.000,3.000,0,'2018-03-06 10:11:51','member14','::1'),(6,5,10,'萝卜牛杂饭',NULL,2,0.000,10.000,2.000,0,'2018-03-06 10:11:51','member14','::1');
/*!40000 ALTER TABLE `ShopOrderItem` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2018-03-06 10:14:20


--add by wwb
DROP TABLE IF EXISTS `ShopComment`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ShopComment` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `ShopId` int(32) DEFAULT NULL,
  `Content` varchar(255) DEFAULT NULL,
  `MemberId` int(32) DEFAULT NULL,  
  `Grade` decimal(10,1) DEFAULT '0.0',
  `KeyWord` varchar(45) DEFAULT NULL,
  --`PictureId1` int(32) DEFAULT NULL,  
  --`PictureId2` int(32) DEFAULT NULL,  
  --`PictureId3` int(32) DEFAULT NULL,  
  `IsDel` int(11) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  `AddIp` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

DROP TABLE IF EXISTS `ShopCommentPicture`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ShopCommentPicture` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `ShopCommentId` int(32) DEFAULT NULL,
  `SaveFileName` varchar(45) DEFAULT NULL,
  `MemberId` int(32) DEFAULT NULL,  
  `IsDel` int(11) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  `AddIp` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

DROP TABLE IF EXISTS `ShopPayInfo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ShopPayInfo` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `ShopId` int(32) DEFAULT NULL,
  `ShopFlag` varchar(128) DEFAULT NULL,
  `PayWay` int(32) DEFAULT NULL,
  `AppId` int(32) DEFAULT NULL,
  `MchId` varchar(128) DEFAULT NULL,
  `SecretKey` varchar(128) DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  `AddUser` varchar(45) DEFAULT NULL,
  `AddIp` varchar(45) DEFAULT NULL,
  `IsEnable` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

DROP TABLE IF EXISTS `ShopOrderReceiver`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ShopOrderReceiver` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `ShopId` int(32) DEFAULT NULL,
  `ReceiverOpenId` varchar(128) DEFAULT NULL,
  `NickName` nvarchar(128) DEFAULT NULL,
  `Headimgurl` varchar(256) DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  `IsUsed` int(11) DEFAULT NULL,
  `AddUser` varchar(45) DEFAULT NULL,
  `AddIp` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;


DROP TABLE IF EXISTS `AccessToken`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `AccessToken` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `Token` varchar(128) DEFAULT NULL,
  `ExpireDate` datetime DEFAULT NULL,
  `TokenType` int(32) DEFAULT 0,
  `IsDel` int(11) DEFAULT NULL,
  `AddUser` varchar(45) DEFAULT NULL,
  `AddIp` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;


DROP TABLE IF EXISTS `ShopOrderMoneyOff`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ShopOrderMoneyOff` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `ShopId` int(32) DEFAULT NULL,
  `StartDate` datetime DEFAULT NULL,
  `EndDate` datetime DEFAULT NULL,
  `Name` varchar(128) DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  `AddUser` varchar(45) DEFAULT NULL,
  `AddIp` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;


DROP TABLE IF EXISTS `ShopOrderMoneyOffRule`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `ShopOrderMoneyOffRule` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `MoneyOffId` int(32) DEFAULT 0,
  `FullAmount` int(32) DEFAULT 0,
  `Discount` int(32) DEFAULT 0,
  `IsDel` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;


CREATE TABLE `ShopOrderMoneyOffCache` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `ShopId` int(32) DEFAULT NULL,
  `MoneyOffId` int(32) DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;


CREATE TABLE `MemberAddress` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `Name` varchar(10) DEFAULT NULL,
  `Sex` varchar(4) DEFAULT NULL,
  `Detail` varchar(128) DEFAULT NULL,
  `Phone` varchar(16) DEFAULT NULL,
  `IsDel` int(11) DEFAULT 0,
  `IsUsed` int(11) DEFAULT 1,
  `AddIp` varchar(45) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;


CREATE TABLE `ShopTakeOutInfo` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `ShopId` int(32) DEFAULT 0,
  `Area` double DEFAULT NULL,
  `MinAmount` int(32) DEFAULT 0,
  `BoxFee` int(32) DEFAULT 0,
  `DeliveryFee` int(32) DEFAULT 0,
  `AutoTakeOrdre` int(11) DEFAULT 1,
  `AutoPrint` int(11) DEFAULT 1,
  `IsDel` int(11) DEFAULT 0,
  `AddIp` varchar(45) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`);
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;


CREATE TABLE `ShopSelfHelpInfo` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `ShopId` int(32) DEFAULT 0,
  `BoxFee` int(32) DEFAULT 0,
  `HasTakeOut` int(11) DEFAULT 0,
  `IsDel` int(11) DEFAULT 0,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;

CREATE TABLE `ShopOrderOtherFee` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `BoxFee` int(32) DEFAULT NULL,
  `DeliveryFee` int(32) DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;

-- 发票抬头表
CREATE TABLE `MemberInvoiceTitle` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `MemberId` int(32) NOT NULL,
  `Type` int(32) DEFAULT 1,
  `MemberInvoiceTitleName`  nvarchar(128)  NOT NULL,
  `BuyerNumber`  nvarchar(45)  NOT NULL,
  `EnterpriseAddress`  nvarchar(128) DEFAULT NULL,
  `Tel`  nvarchar(11) DEFAULT NULL,
  `BankDeposit`  nvarchar(128) DEFAULT NULL,
  `BankAccount`  nvarchar(45) DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;

-- 发票请求表
CREATE TABLE `MemberInvoiceRequest` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `MemberId` int(32) NOT NULL,
  `ShopOrderId` int(32) NOT NULL,
  `MemberInvoiceTitleId` int(32)  NOT NULL,
  `ShopId` int(32)  NOT NULL,
  `CreateTime` datetime NOT NULL,
  `State` int(32) DEFAULT 1,
  `IsDel` int(11) DEFAULT 0,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;


CREATE TABLE `ShopComboCategory` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `ShopBrandId` int(32) DEFAULT NULL,
  `Name` nvarchar(45) DEFAULT NULL,
  `IsDel` int(11) DEFAULT 0,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;



CREATE TABLE `ShopCommodityComboItem` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `ComboId` int(32) DEFAULT NULL,
  `ComboItemName` nvarchar(45) DEFAULT NULL,
  `CommodityIds` nvarchar(128) DEFAULT NULL,
  `IsDel` int(11) DEFAULT 0,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;




CREATE TABLE `ShopCommodityCombo` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `ShopId` int(32) DEFAULT NULL,
  `ComboName` nvarchar(45) DEFAULT NULL,
  `SalePrice` int(32) DEFAULT NULL,
  `MarketPrice` int(32) DEFAULT NULL,
    `AddIp` varchar(45) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  `IsDel` int(11) DEFAULT 0,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `bannerconfiguration`;
CREATE TABLE `bannerconfiguration` (
  `Id` int(10) NOT NULL auto_increment,
  `Name` varchar(255) default NULL,
  `Url` varchar(255) default NULL,
  `Link` varchar(255) default NULL,
  `IsShow` int(1) default NULL,
  `Sorting` int(6) default NULL,
  `IsDel` int(1) default NULL,
  PRIMARY KEY  (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;



DROP TABLE IF EXISTS `ShopOrderTakeout`;
CREATE TABLE `ShopOrderTakeout` (
  `Id` int(10) NOT NULL auto_increment,
  `ShopOrderId` int(32) default NULL,
  `TakeWay` int(11) default NULL,
  `PickupTime` datetime default NULL,
  `MemberId` int(32) default NULL,
  `Name` varchar(255) default NULL,
  `Sex` varchar(255) default NULL,
  `Address` varchar(255) default NULL,
  `Phone` varchar(255) default NULL,
  `IsDel` int(1) default 0,
  PRIMARY KEY  (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;


DROP TABLE IF EXISTS `WechatOpenTicket`;
CREATE TABLE `WechatOpenTicket` (
  `Id` int(10) NOT NULL auto_increment,
  `AppId` varchar(255) default NULL,
  `OpenTicket` varchar(255) default NULL,
  `IsDel` int(1) default 0,
  PRIMARY KEY  (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;


CREATE TABLE `conglomerationactivity` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `IsDel` int(11) DEFAULT '0',
  `CreateTime` datetime NOT NULL,
  `CoverPortal` varchar(255) NOT NULL,
  `ActivityName` varchar(255) NOT NULL,
  `Context` longtext NOT NULL,
  `ActivityEndTime` datetime DEFAULT NULL,
  `ActivityBeginTime` datetime DEFAULT NULL,
  `BrowseNumber` int(11) DEFAULT '0',
  `DeliveryTakeTheirBeginTimeMD` datetime DEFAULT NULL,
  `DeliveryTakeTheirEndTimeMD` datetime DEFAULT NULL,
  `ActivityDeliveryFee` int(32) NOT NULL,
  `ShopId` int(32) DEFAULT NULL,
  `DeliveryTakeTheirBeginTimeHM` datetime DEFAULT NULL,
  `DeliveryTakeTheirEndTimeHM` datetime DEFAULT NULL,
  `MarketPrice` int(32) DEFAULT NULL,
  `ConglomerationCountdown` int(11) DEFAULT '30',
  `Intro` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=29 DEFAULT CHARSET=utf8


CREATE TABLE `ConglomerationActivityType` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `IsDel` int(11) DEFAULT 0,
  `CreateTime` datetime NOT NULL,
  `ConglomerationMembers` int(11)  NOT NULL,
  `ConglomerationPrice` int(32)  NOT NULL,
  `ConglomerationActivityId` int(32)  NOT NULL,
    `TypeDescribe` varchar(400)  ,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;


CREATE TABLE `ConglomerationCommodity` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `IsDel` int(11) DEFAULT 0,
  `CreateTime` datetime NOT NULL,
  `Name`  varchar(255)  NOT NULL,
  `MarketPrice` int(32)  NOT NULL,
  `Summary`  varchar(4000)  NOT NULL,
  `ConglomerationActivityId` int(32)  NOT NULL,
   `ShopId`  int(32)  NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;



CREATE TABLE `conglomerationorder` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `IsDel` int(11) DEFAULT '0',
  `CreateTime` datetime NOT NULL,
  `OrderNumber` varchar(255) NOT NULL,
  `Type` int(11) DEFAULT '0',
  `ConglomerationSetUpId` int(32) NOT NULL,
  `PickupCode` varchar(255) DEFAULT NULL,
  `Amount` int(32) NOT NULL,
  `Payment` int(32) DEFAULT NULL,
  `PayTime` datetime DEFAULT NULL,
  `RefundTime` datetime DEFAULT NULL,
  `FinishTime` datetime DEFAULT NULL,
  `ShopId` int(32) NOT NULL,
  `MemberId` int(32) NOT NULL,
  `AddUser` varchar(255) DEFAULT NULL,
  `AddIp` varchar(255) DEFAULT NULL,
  `ConglomerationActivityId` int(32) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


CREATE TABLE `conglomerationsetup` (
  `Id` int(32) NOT NULL AUTO_INCREMENT ,
  `IsDel` int(11) DEFAULT '0',
  `CreateTime` datetime NOT NULL,
  `MemberNumber` int(11) NOT NULL,
  `CurrentMemberNumber` int(11) NOT NULL,
  `Status` int(11) NOT NULL,
  `ConglomerationActivityId` int(32) NOT NULL,
  `EndTime` datetime NOT NULL,
  `ConglomerationActivityTypeId` int(32) NOT NULL,
  `MemberId` int(32) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;


CREATE TABLE `ShopOrderComboItem` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `Pid` int(32) default NULL,
  `CommodityId` int(32) default NULL,
  `IsDel` int(11) DEFAULT 0,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;



CREATE TABLE `ConglomerationActivityPickingSetting` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `IsDel` int(11) DEFAULT 0,
  `CreateTime` datetime NOT NULL,
  `Type` int(11)  NOT NULL,
  `ConglomerationActivityId` int(32)  NOT NULL,
   `PickingSettingName`  varchar(255)  NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;



CREATE TABLE `ConglomerationParticipation` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `IsDel` int(11) DEFAULT 0,
  `CreateTime` datetime NOT NULL,
  `AvatarUrl` varchar(300)  NOT NULL,
  `NickName` varchar(255)  NOT NULL,
  `ConglomerationSetUpId` int(32)  NOT NULL,
  `Role` int(11)  NOT NULL,
  `MemberId` int(32)  NOT NULL,
  `ConglomerationOrderId` int(32)  NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;



CREATE TABLE `conglomerationexpress` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `IsDel` int(11) DEFAULT '0',
  `CreateTime` datetime NOT NULL,
  `ExpressSingle` varchar(300) DEFAULT NULL,
  `MemberAddressId` int(32) NOT NULL,
  `DeliveryTakeTheirBeginTimeMD` datetime NOT NULL,
  `DeliveryTakeTheirEndTimeMD` datetime NOT NULL,
  `DeliveryTakeTheirBeginTimeHM` datetime NOT NULL,
  `DeliveryTakeTheirEndTimeHM` datetime NOT NULL,
  `Name` varchar(45) DEFAULT NULL,
  `Sex` varchar(45) DEFAULT NULL,
  `Address` varchar(425) DEFAULT NULL,
  `Phone` varchar(45) DEFAULT NULL,
  `Status` int(11) DEFAULT NULL,
  `ShopConglomerationOrderId` int(32) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;




CREATE TABLE `ShopOrderSelfHelp` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `IsDel` int(11) DEFAULT 0,
  `Number` varchar(45)  NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;



CREATE TABLE `CodeForShopOrderSelfHelp` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `IsDel` int(11) DEFAULT 0,
  `ShopId` int(32) NOT NULL,
  `CurrentNumber` int(32) DEFAULT 0,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;


CREATE TABLE `ShopTemplateMessageInfo` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `IsDel` int(11) DEFAULT 0,
  `CreateTime` datetime NOT NULL,
  `TemplateId` varchar(100) DEFAULT NULL,
  `Title` varchar(100) DEFAULT NULL,
  `Content` varchar(300) DEFAULT NULL,
  `Example` varchar(300) DEFAULT NULL,
  `ShopId` int(32) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;


CREATE TABLE `ShopMember` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `IsDel` tinyint(11) DEFAULT 0,
  `Name` varchar(60) DEFAULT NULL,
  `Sex` varchar(10) DEFAULT NULL,
  `Phone` varchar(20) DEFAULT NULL,
  `Credits` int(32) DEFAULT 0,
  `Balance` int(32) DEFAULT 0,
  `ShopId` int(32) NOT NULL,
  `MemberId` int(32) NOT NULL,
  `BirthDay` datetime NOT NULL,
  `AddTime` datetime NOT NULL,
  `AddIp` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;



-- 商铺会员等级
CREATE TABLE `ShopMemberLevel` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `IsDel` tinyint(11) DEFAULT 0,
  `ShopId` int(32) NOT NULL,
  `AddTime` datetime NOT NULL,
  `AddIp` varchar(20) DEFAULT NULL,
  `LevelName` varchar(100) DEFAULT NULL,
  `MemberLevel` varchar(100) DEFAULT NULL,
  `MaxIntegral` int(32) DEFAULT NULL,
  `MinIntegral` int(32) DEFAULT NULL,
  `Discount` double(16,2) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;



-- 自定义金额充值设置
CREATE TABLE `ShopCustomTopUpSet` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `IsDel` tinyint(11) DEFAULT 0,
  `ShopId` int(32) NOT NULL,
  `AddTime` datetime NOT NULL,
  `AddIp` varchar(20) DEFAULT NULL,
  `StartAmount` int(32) NOT NULL,
  `MeetAmount` int(32) NOT NULL,
  `Additional`  double(16,2) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;


-- 会员设置
CREATE TABLE `ShopMemberSet` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `IsDel` tinyint(11) DEFAULT 0,
  `ShopId` int(32) NOT NULL,
  `AddTime` datetime NOT NULL,
  `AddIp` varchar(20) DEFAULT NULL,
  `IsSavaLevel` tinyint(1) NOT NULL,
  `IsTopUpDiscount` tinyint(1) NOT NULL,
  `IsConsumeIntegral` tinyint(1) NOT NULL,
  `IsTopUpIntegral` tinyint(1) NOT NULL,
  `ConsumeAmount` int(100) NOT NULL,
  `GetIntegral` int(32) NOT NULL,

  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;



-- 固定金额充值设置
CREATE TABLE `ShopTopUpSet` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `IsDel` tinyint(11) DEFAULT 0,
  `ShopId` int(32) NOT NULL,
  `AddTime` datetime NOT NULL,
  `AddIp` varchar(20) DEFAULT NULL,
  `FixationTopUp` int(100) NOT NULL,
  `FixationTopUpAmount` int(100) NOT NULL,
  `PresentedAmount` int(100) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;


ALTER TABLE shopbrandcommodity add UseMemberPrice TINYINT DEFAULT 0



CREATE TABLE `ShopBrandCombo` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `ShopBrandId` int(32) DEFAULT NULL,
  `Flag` varchar(45) DEFAULT NULL,
  `Name` varchar(45) DEFAULT NULL,
  `Price` decimal(10,3) DEFAULT '0.000',
  `Unit` varchar(45) DEFAULT NULL,
  `Summary` varchar(255) DEFAULT NULL,
  `SalesForMonth` int(32) DEFAULT '0',
  `Upvote` int(32) DEFAULT '0',
  `ComboType` int(32) DEFAULT 0,
  `IsRecommand` tinyint DEFAULT 0,
  `Cover` varchar(255) DEFAULT NULL,
  `IsScanCode` tinyint DEFAULT 0,
  `IsTakeout` tinyint DEFAULT 0,
  `IsSelfOrder` tinyint DEFAULT 0,
  `IsDel` tinyint DEFAULT 0,
  `AddTime` datetime DEFAULT NULL,
  `AddIp` varchar(45) DEFAULT NULL,
  `AddUser` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;





CREATE TABLE `ShopBrandFixComboItem` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `ComboId` int(32) DEFAULT NULL,
  `CommodityName` varchar(45) DEFAULT NULL,
  `Count` varchar(45) DEFAULT NULL,
  `SalePrice` decimal(10,2) DEFAULT '0.00',
  `Sku` varchar(45) DEFAULT NULL,
  `IsDel` tinyint DEFAULT 0,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;