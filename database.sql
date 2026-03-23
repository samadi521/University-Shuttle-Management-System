CREATE DATABASE shuttle_db;
USE shuttle_db;

CREATE TABLE users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100),
    email VARCHAR(100) UNIQUE,
    password VARCHAR(255),
    role ENUM('admin','driver','student')
);

CREATE TABLE trips (
    id INT AUTO_INCREMENT PRIMARY KEY,
    driver_id INT,
    start_location VARCHAR(255),
    end_location VARCHAR(255),
    price DECIMAL(10,2),
    available_seats INT,
    status VARCHAR(50) DEFAULT 'active'
);

CREATE TABLE locations (
    id INT AUTO_INCREMENT PRIMARY KEY,
    driver_id INT,
    latitude DOUBLE,
    longitude DOUBLE,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE notifications (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT,
    message TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);


INSERT INTO users (name,email,password,role) VALUES
('Admin','admin@test.com','$2b$10$mD25f36en42TFQXy22VUHu2QXunuCvwjOX/XlST0Dm2W3VUyadj6O','admin'),
('Driver','driver@test.com','$2b$10$mD25f36en42TFQXy22VUHu2QXunuCvwjOX/XlST0Dm2W3VUyadj6O','driver'),
('Student','student@test.com','$2b$10$mD25f36en42TFQXy22VUHu2QXunuCvwjOX/XlST0Dm2W3VUyadj6O','student');
CREATE TABLE bookings (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT,
    trip_id INT,
    seats INT DEFAULT 1,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);