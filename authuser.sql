-- phpMyAdmin SQL Dump
-- version 5.2.3
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1:3306
-- Létrehozás ideje: 2026. Nov 11. 06:46
-- Kiszolgáló verziója: 8.4.7
-- PHP verzió: 8.3.28

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Adatbázis: `authuser`
--
CREATE DATABASE IF NOT EXISTS `authuser` DEFAULT CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci;
USE `authuser`;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `roles`
--

DROP TABLE IF EXISTS `roles`;
CREATE TABLE IF NOT EXISTS `roles` (
  `Id` char(36) NOT NULL,
  `Name` varchar(40) DEFAULT NULL,
  `RegTime` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `userrole`
--

DROP TABLE IF EXISTS `userrole`;
CREATE TABLE IF NOT EXISTS `userrole` (
  `UserId` char(36) NOT NULL,
  `RoleId` char(36) NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `UserId` (`UserId`,`RoleId`),
  KEY `RoleId` (`RoleId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `users`
--

DROP TABLE IF EXISTS `users`;
CREATE TABLE IF NOT EXISTS `users` (
  `Id` char(36) NOT NULL,
  `Name` varchar(40) DEFAULT NULL,
  `Email` varchar(50) DEFAULT NULL,
  `Password` text,
  `RegTime` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

--
-- A tábla adatainak kiíratása `users`
--

INSERT INTO `users` (`Id`, `Name`, `Email`, `Password`, `RegTime`) VALUES
('d3e1943a-e774-4d64-bef0-6f9acb1a9eae', 'Charlie Kirk', 'Kurky@gmail.com', 'Kirkike', '2026-11-04 10:40:34');

--
-- Megkötések a kiírt táblákhoz
--

--
-- Megkötések a táblához `userrole`
--
ALTER TABLE `userrole`
  ADD CONSTRAINT `userrole_ibfk_1` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `userrole_ibfk_2` FOREIGN KEY (`RoleId`) REFERENCES `roles` (`Id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
