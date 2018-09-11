-- MySQL dump 10.13  Distrib 5.7.17, for Win64 (x86_64)
--
-- Host: 119.147.144.104    Database: wechatCore
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

--
-- Table structure for table `CustomerMessage`
--

DROP TABLE IF EXISTS `CustomerMessage`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `CustomerMessage` (
  `Id` bigint(64) NOT NULL AUTO_INCREMENT,
  `FromUser` varchar(45) CHARACTER SET utf8 DEFAULT NULL,
  `Content` varchar(2000) CHARACTER SET utf8 DEFAULT NULL,
  `Time` datetime DEFAULT NULL,
  `ToUser` varchar(45) DEFAULT NULL,
  `ChatFlag` varchar(45) DEFAULT NULL,
  `MemberIsRead` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=606 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CustomerMessage`
--

LOCK TABLES `CustomerMessage` WRITE;
/*!40000 ALTER TABLE `CustomerMessage` DISABLE KEYS */;
/*!40000 ALTER TABLE `CustomerMessage` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CustomerPhone`
--

DROP TABLE IF EXISTS `CustomerPhone`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `CustomerPhone` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `OpenId` varchar(45) DEFAULT NULL,
  `Phone` varchar(45) DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  `Status` int(32) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CustomerPhone`
--

LOCK TABLES `CustomerPhone` WRITE;
/*!40000 ALTER TABLE `CustomerPhone` DISABLE KEYS */;
/*!40000 ALTER TABLE `CustomerPhone` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CustomerSession`
--

DROP TABLE IF EXISTS `CustomerSession`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `CustomerSession` (
  `Id` bigint(64) NOT NULL AUTO_INCREMENT,
  `OpenId` varchar(45) DEFAULT NULL,
  `Time` datetime DEFAULT NULL,
  `Worker` varchar(1000) DEFAULT NULL,
  `Status` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CustomerSession`
--

LOCK TABLES `CustomerSession` WRITE;
/*!40000 ALTER TABLE `CustomerSession` DISABLE KEYS */;
/*!40000 ALTER TABLE `CustomerSession` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `CustomerSmsValiCodeTask`
--

DROP TABLE IF EXISTS `CustomerSmsValiCodeTask`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
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
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CustomerSmsValiCodeTask`
--

LOCK TABLES `CustomerSmsValiCodeTask` WRITE;
/*!40000 ALTER TABLE `CustomerSmsValiCodeTask` DISABLE KEYS */;
/*!40000 ALTER TABLE `CustomerSmsValiCodeTask` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `MemberWeChatBindTask`
--

DROP TABLE IF EXISTS `MemberWeChatBindTask`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `MemberWeChatBindTask` (
  `Id` bigint(64) NOT NULL AUTO_INCREMENT,
  `Code` varchar(45) DEFAULT NULL,
  `MemberId` int(11) DEFAULT NULL,
  `OpenId` varchar(45) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  `AddIp` varchar(45) DEFAULT NULL,
  `Status` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `MemberWeChatBindTask`
--

LOCK TABLES `MemberWeChatBindTask` WRITE;
/*!40000 ALTER TABLE `MemberWeChatBindTask` DISABLE KEYS */;
INSERT INTO `MemberWeChatBindTask` VALUES (14,'98',1,'','2018-02-27 16:47:46','14.120.107.34',0),(15,'99',1,'','2018-02-27 17:03:40','14.120.107.34',0);
/*!40000 ALTER TABLE `MemberWeChatBindTask` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `MemberWeChatLoginTask`
--

DROP TABLE IF EXISTS `MemberWeChatLoginTask`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `MemberWeChatLoginTask` (
  `Id` bigint(64) NOT NULL AUTO_INCREMENT,
  `ClientId` varchar(45) DEFAULT NULL,
  `Code` varchar(45) DEFAULT NULL,
  `OpenId` varchar(45) DEFAULT NULL,
  `AddTime` datetime DEFAULT NULL,
  `AddIp` varchar(45) DEFAULT NULL,
  `Status` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=48 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `MemberWeChatLoginTask`
--

LOCK TABLES `MemberWeChatLoginTask` WRITE;
/*!40000 ALTER TABLE `MemberWeChatLoginTask` DISABLE KEYS */;
INSERT INTO `MemberWeChatLoginTask` VALUES (47,'xepcxwjz28qxdp16qnaavnhvptifncqk2fvntxyhopjtx','97','','2018-02-27 16:30:13','14.120.107.34',0);
/*!40000 ALTER TABLE `MemberWeChatLoginTask` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `MemberWechat`
--

DROP TABLE IF EXISTS `MemberWechat`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `MemberWechat` (
  `Id` bigint(64) NOT NULL AUTO_INCREMENT,
  `MemberId` int(11) DEFAULT NULL,
  `OpenId` varchar(100) CHARACTER SET utf8 DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=35 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `MemberWechat`
--

LOCK TABLES `MemberWechat` WRITE;
/*!40000 ALTER TABLE `MemberWechat` DISABLE KEYS */;
INSERT INTO `MemberWechat` VALUES (29,9,'o5pAb5B8cxRsewJBqJdsoWnrqdtQ',0),(30,10,'o5pAb5DSl4KqZoQ6CMir0akWDY20',0),(31,11,'o5pAb5GEv8j6n1WTaifvpeViHf5s',0),(32,12,'o5pAb5PyF4f7aUabH4lRcoOvGQ-o',0),(33,13,'o5pAb5K5nu-KCoD24z4vpTCOj7fA',0),(34,14,'oNtLks6Gq9oh6nEa8vSUjoc-cvvM',0);
/*!40000 ALTER TABLE `MemberWechat` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `RequestMsgData`
--

DROP TABLE IF EXISTS `RequestMsgData`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
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
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `RequestMsgData`
--

LOCK TABLES `RequestMsgData` WRITE;
/*!40000 ALTER TABLE `RequestMsgData` DISABLE KEYS */;
/*!40000 ALTER TABLE `RequestMsgData` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `RobotMessage`
--

DROP TABLE IF EXISTS `RobotMessage`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `RobotMessage` (
  `Id` int(32) NOT NULL AUTO_INCREMENT,
  `QuestionType` int(32) DEFAULT NULL,
  `Question` varchar(255) CHARACTER SET utf8 DEFAULT NULL,
  `Answer` varchar(255) CHARACTER SET utf8 DEFAULT NULL,
  `IsDel` int(11) DEFAULT NULL,
  `Status` int(32) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `RobotMessage`
--

LOCK TABLES `RobotMessage` WRITE;
/*!40000 ALTER TABLE `RobotMessage` DISABLE KEYS */;
/*!40000 ALTER TABLE `RobotMessage` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `WechatQRScene`
--

DROP TABLE IF EXISTS `WechatQRScene`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `WechatQRScene` (
  `Id` bigint(64) NOT NULL AUTO_INCREMENT,
  `SceneId` int(11) DEFAULT NULL,
  `QrCodeTicket` varchar(100) CHARACTER SET utf8 DEFAULT NULL,
  `Category` varchar(45) DEFAULT NULL,
  `Status` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=100 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `WechatQRScene`
--

LOCK TABLES `WechatQRScene` WRITE;
/*!40000 ALTER TABLE `WechatQRScene` DISABLE KEYS */;
INSERT INTO `WechatQRScene` VALUES (97,97,'gQHm7zwAAAAAAAAAAS5odHRwOi8vd2VpeGluLnFxLmNvbS9xLzAyTGdPTU5sU2liQVAxeDBvbHhxMXYAAgQUF5VaAwQsAQAA','Login',0),(98,98,'gQHc7zwAAAAAAAAAAS5odHRwOi8vd2VpeGluLnFxLmNvbS9xLzAyUUtaNE1JU2liQVAxeHRzbGhxMXAAAgQxG5VaAwQsAQAA','BindMember',0),(99,99,'gQEo8DwAAAAAAAAAAS5odHRwOi8vd2VpeGluLnFxLmNvbS9xLzAya2hYWU5MU2liQVAxd293bDFxMUEAAgTsHpVaAwQsAQAA','BindMember',0);
/*!40000 ALTER TABLE `WechatQRScene` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2018-03-06 10:15:10
