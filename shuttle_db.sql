-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Mar 26, 2026 at 06:42 PM
-- Server version: 8.0.44
-- PHP Version: 8.0.30

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `shuttle_db`
--

-- --------------------------------------------------------

--
-- Table structure for table `bookings`
--

CREATE TABLE `bookings` (
  `id` int NOT NULL,
  `user_id` int DEFAULT NULL,
  `trip_id` int DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `bookings`
--

INSERT INTO `bookings` (`id`, `user_id`, `trip_id`, `created_at`) VALUES
(1, 1, 1, '2026-03-26 15:08:52'),
(2, 1, 1, '2026-03-26 15:31:42'),
(3, 1, 1, '2026-03-26 16:56:04'),
(4, 1, 4, '2026-03-26 17:36:01');

-- --------------------------------------------------------

--
-- Table structure for table `buses`
--

CREATE TABLE `buses` (
  `id` int NOT NULL,
  `driver_id` int DEFAULT NULL,
  `start_location` varchar(100) DEFAULT NULL,
  `end_location` varchar(100) DEFAULT NULL,
  `price` decimal(10,2) DEFAULT NULL,
  `total_seats` int DEFAULT NULL,
  `available_seats` int DEFAULT NULL,
  `status` enum('active','cancelled','delayed') DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `buses`
--

INSERT INTO `buses` (`id`, `driver_id`, `start_location`, `end_location`, `price`, `total_seats`, `available_seats`, `status`) VALUES
(1, 1, 'chilaw', 'colombo', 220.00, 20, 1, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `locations`
--

CREATE TABLE `locations` (
  `id` int NOT NULL,
  `bus_id` int DEFAULT NULL,
  `latitude` double DEFAULT NULL,
  `longitude` double DEFAULT NULL,
  `updated_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- --------------------------------------------------------

--
-- Table structure for table `notifications`
--

CREATE TABLE `notifications` (
  `id` int NOT NULL,
  `message` text,
  `bus_id` int DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `type` varchar(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `notifications`
--

INSERT INTO `notifications` (`id`, `message`, `bus_id`, `created_at`, `type`) VALUES
(1, 'Bus 1 is CANCELLED ❌', 1, '2026-03-26 07:59:15', 'Cancelled');

-- --------------------------------------------------------

--
-- Table structure for table `trips`
--

CREATE TABLE `trips` (
  `id` int NOT NULL,
  `driver_id` int DEFAULT NULL,
  `start_location` varchar(255) DEFAULT NULL,
  `end_location` varchar(255) DEFAULT NULL,
  `price` double DEFAULT NULL,
  `available_seats` int DEFAULT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `start_lat` double DEFAULT NULL,
  `start_lng` double DEFAULT NULL,
  `end_lat` double DEFAULT NULL,
  `end_lng` double DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `trips`
--

INSERT INTO `trips` (`id`, `driver_id`, `start_location`, `end_location`, `price`, `available_seats`, `created_at`, `start_lat`, `start_lng`, `end_lat`, `end_lng`) VALUES
(1, 1, 'we', 'ew', 12, 9, '2026-03-26 11:42:50', NULL, NULL, NULL, NULL),
(2, 1, 'we', 'ew', 12, 12, '2026-03-26 11:42:55', NULL, NULL, NULL, NULL),
(3, 1, 'chilaw', 'colombo', 230, 23, '2026-03-26 15:31:06', NULL, NULL, NULL, NULL),
(4, 1, 'chilaw', 'colombo', 240, 53, '2026-03-26 16:06:03', NULL, NULL, NULL, NULL),
(5, 1, 'ew', 'ewe', 23, 34, '2026-03-26 17:35:18', NULL, NULL, NULL, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `id` int NOT NULL,
  `email` varchar(100) DEFAULT NULL,
  `password` varchar(255) DEFAULT NULL,
  `role` enum('student','driver','admin') DEFAULT NULL,
  `name` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`id`, `email`, `password`, `role`, `name`) VALUES
(1, 'shimron@gmail.com', '$2b$10$9wyUn5xj62Anm6i/ggbkHe8IEAqUE0koDStThBNb1XJgi942IkZKW', 'student', ''),
(2, 'admin@gmail.com', '$2b$10$.fT9NZSnsWnF4f0YiamihOgG9KHEX5c/Hr92A/yViI1T6aLKY8l.O', 'admin', ''),
(3, 'driver@gmail.com', '$2b$10$Ej7IDtbAM/B7oBLwpz3mtOIuoCbcUM36aMOzdujQMt7JGG1ZaQwtS', 'driver', ''),
(4, 'student@gmail.com', '$2b$10$hS6UftSzYGar3sSiCksig.n3sfYsksuw/zTU9ynE3tFnteD5E9qYu', 'student', 'shimron'),
(5, 'studen@tgmail.com', '$2b$10$X/NqTYCxvUCW8MHJjrPL1ehGMWU0a4BPI3AakbWgeUakyryidYbGO', 'student', 'shim'),
(6, 'as', '$2b$10$z/QLDNeREEz9c5.3tgFss.JtKAr389IXBT55fnNNRs0a4jMGOgm4C', 'driver', 'we'),
(7, 'sk@gmail.com', '$2b$10$9t84ZmCqNPcMOkuZR4LpFOHd4UILia2uWjZR0GItee85miY/Si8aK', 'student', '1'),
(8, 'sd', '$2b$10$DpQ1w/W1hECb/q.hn1v/Y.SGdqmBPxv9em4rH8lUVVxCjNNqN76XG', 'student', 'sd'),
(9, 'qw', '$2b$10$1.8K8zkLDzONSi9vF55kvufEGiTO2rhvW0eSERdM0GnMwPOG6xZU6', 'driver', '1'),
(10, 'sammani@gmail.com', '$2b$10$POHHwtdKkpsZwp/LUUU5uuJYEM63fKo0GP6xp9dxxcdtQYXNydT4m', 'student', '2'),
(11, 'ds@gmail.com', '$2b$10$6BRU/GmU103XLK9OJ.ub6u8J98LeYEs3ayB5itWY.AyczAgbphzrG', 'student', 'sd'),
(12, 'er@gmail.com', '$2b$10$2/o7cy.hxvkXMNN7dEOEjei1Mgwk5dbrIyVwybp.YqRcT5.8BgyZq', 'driver', 'ds'),
(13, 'ds', '$2b$10$42L6g8L0Fo/h8XlkC9Nvn.wZxk26W0s6sU2GIt5eUPkCfIxOL2Lxe', 'student', 'shim');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `bookings`
--
ALTER TABLE `bookings`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `buses`
--
ALTER TABLE `buses`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `locations`
--
ALTER TABLE `locations`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `notifications`
--
ALTER TABLE `notifications`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `trips`
--
ALTER TABLE `trips`
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
-- AUTO_INCREMENT for table `bookings`
--
ALTER TABLE `bookings`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `buses`
--
ALTER TABLE `buses`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT for table `locations`
--
ALTER TABLE `locations`
  MODIFY `id` int NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `notifications`
--
ALTER TABLE `notifications`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT for table `trips`
--
ALTER TABLE `trips`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
