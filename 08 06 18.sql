-- --------------------------------------------------------
-- Hôte :                        127.0.0.1
-- Version du serveur:           8.0.11 - MySQL Community Server - GPL
-- SE du serveur:                Win64
-- HeidiSQL Version:             9.5.0.5196
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;


-- Export de la structure de la base pour supfile
CREATE DATABASE IF NOT EXISTS `supfile` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `supfile`;

-- Export de la structure de la table supfile. filesystem
CREATE TABLE IF NOT EXISTS `filesystem` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `userId` int(10) unsigned NOT NULL,
  `shared` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `name` text NOT NULL,
  `parent` int(10) unsigned NOT NULL DEFAULT '0',
  `isDir` tinyint(1) unsigned NOT NULL DEFAULT '0',
  `drive` text,
  `size` int(10) unsigned DEFAULT '0',
  KEY `id` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8;

-- Export de données de la table supfile.filesystem : ~3 rows (environ)
DELETE FROM `filesystem`;
/*!40000 ALTER TABLE `filesystem` DISABLE KEYS */;
/*!40000 ALTER TABLE `filesystem` ENABLE KEYS */;

-- Export de la structure de la table supfile. user
CREATE TABLE IF NOT EXISTS `user` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `mail` text NOT NULL,
  `stockage` int(10) unsigned NOT NULL,
  `stockagemax` int(10) unsigned NOT NULL DEFAULT '0',
  `password` text NOT NULL,
  KEY `id` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;

-- Export de données de la table supfile.user : ~0 rows (environ)
DELETE FROM `user`;
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
/*!40000 ALTER TABLE `user` ENABLE KEYS */;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
