-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: localhost
-- Generation Time: Jun 14, 2025 at 05:01 PM
-- Server version: 10.11.9-MariaDB
-- PHP Version: 8.2.28

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `bobhoogenboom`
--

-- --------------------------------------------------------

--
-- Table structure for table `gdv_games`
--

CREATE TABLE `gdv_games` (
  `id` int(11) NOT NULL,
  `fortnite` varchar(30) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_520_ci NOT NULL,
  `minecraft` varchar(25) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  `wow` varchar(25) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  `terraria` varchar(25) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  `finalfantasyxiv` varchar(25) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- --------------------------------------------------------

--
-- Table structure for table `gdv_scores`
--

CREATE TABLE `gdv_scores` (
  `id` int(11) NOT NULL,
  `gameid` int(11) NOT NULL,
  `playerid` int(11) NOT NULL,
  `serverid` int(11) NOT NULL,
  `score` int(11) NOT NULL,
  `achievedate` date NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

--
-- Dumping data for table `gdv_scores`
--

INSERT INTO `gdv_scores` (`id`, `gameid`, `playerid`, `serverid`, `score`, `achievedate`) VALUES
(1, 1, 1, 1, 2025, '2025-06-05'),
(2, 2, 2, 2, 2000, '2025-05-06'),
(3, 2, 2, 2, 2000, '2025-05-06'),
(4, 3, 2, 2, 2000, '2025-05-06'),
(5, 1, 5, 2, 73, '2025-05-13');

-- --------------------------------------------------------

--
-- Table structure for table `gdv_servers`
--

CREATE TABLE `gdv_servers` (
  `id` int(11) NOT NULL,
  `serverid` int(11) NOT NULL,
  `password` varchar(30) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_520_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

--
-- Dumping data for table `gdv_servers`
--

INSERT INTO `gdv_servers` (`id`, `serverid`, `password`) VALUES
(1, 1, 'DitIsServer01!'),
(2, 2, 'DitIsServer02!'),
(3, 3, 'DitIsServer03!'),
(4, 4, 'DitIsServer04!'),
(5, 5, 'DitIsServer05!');

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `id` int(11) NOT NULL,
  `email` varchar(30) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  `nickname` varchar(25) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  `password` varchar(30) NOT NULL,
  `geboortedatum` date NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`id`, `email`, `nickname`, `password`, `geboortedatum`) VALUES
(1, 'test@test.nl', 'tester123', 'WachtwoordTest123!', '2000-01-01'),
(2, 'jan@jan.nl', 'JanTheDestroyer234?', 'WachtwoordJan234?', '2001-02-02'),
(3, 'kees@kees.nl', 'KeesTheDestroyer345+', 'WachtwoordKees345+', '2002-03-03'),
(4, 'piet@piet.nl', 'PietTheDestroyer456-', 'WachtwoordPiet456-', '2003-04-04'),
(5, 'mies@mies.nl', 'MiesTheDestroyer567#', 'WachtwoordMies567#', '2004-05-05'),
(6, 'jan@jan.nl', 'JanTheDestroyer234?', 'WachtwoordJan234?', '2001-02-02');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `gdv_games`
--
ALTER TABLE `gdv_games`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `gdv_scores`
--
ALTER TABLE `gdv_scores`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `gdv_servers`
--
ALTER TABLE `gdv_servers`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `gdv_games`
--
ALTER TABLE `gdv_games`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `gdv_scores`
--
ALTER TABLE `gdv_scores`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `gdv_servers`
--
ALTER TABLE `gdv_servers`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
