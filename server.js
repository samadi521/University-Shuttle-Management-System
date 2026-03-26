const express = require('express');
const mysql = require('mysql2');
const cors = require('cors');
const bcrypt = require('bcrypt');
const jwt = require('jsonwebtoken');
const http = require('http');
const { Server } = require('socket.io');

const app = express();
app.use(cors());
app.use(express.json());

const SECRET = "shuttle_secret_key";

const db = mysql.createConnection({
    host: 'localhost',
    user: 'root',
    password: 'admin123',
    database: 'shuttle_db'
});
// 🔥 ADD THIS ABOVE LOGIN
app.post('/auth/register', async (req, res) => {
    const { name, email, password } = req.body;

    if (!name || !email || !password) {
        return res.json({ error: "All fields required" });
    }

    db.query("SELECT * FROM users WHERE email=?", [email], async (err, result) => {

        if (result.length > 0) {
            return res.json({ error: "User already exists" });
        }

        const hash = await bcrypt.hash(password, 10);

        db.query(
            "INSERT INTO users (name,email,password,role) VALUES (?,?,?,?)",
            [name, email, hash, "student"], // ✅ ONLY STUDENT
            (err) => {
                if (err) return res.json(err);

                res.json({ message: "Student registered" });
            }
        );
    });
});
app.post('/login', (req, res) => {
    const { email, password } = req.body;

    db.query("SELECT * FROM users WHERE email=?", [email], async (err, result) => {

        if (result.length === 0) {
            console.log("❌ User not found");
            return res.json({ error: "User not found" });
        }

        const user = result[0];

        // 🔍 DEBUG HERE
        console.log("DB Password:", user.password);
        console.log("Entered Password:", password);
let match = false;

if (user.password.startsWith("$2b$")) {
    // ✅ hashed password
    match = await bcrypt.compare(password, user.password);
} else {
    // ⚠️ old plain text password
    match = password === user.password;

    // 🔄 AUTO CONVERT TO HASH (VERY IMPORTANT)
    if (match) {
        const newHash = await bcrypt.hash(password, 10);

        db.query(
            "UPDATE users SET password=? WHERE id=?",
            [newHash, user.id]
        );

        console.log("🔄 Password upgraded to hash");
    }
}

if (!match) {
    return res.json({ error: "Wrong password" });
}

        const token = jwt.sign(
            { id: user.id, role: user.role },
            SECRET,
            { expiresIn: "1d" }
        );

        console.log("✅ Login success");

        res.json({
            token,
            role: user.role,
            userId: user.id
        });
    });
});
app.post('/add-driver', async (req, res) => {
    const { name, email, password } = req.body;

    const hash = await bcrypt.hash(password, 10);

    db.query(
        "INSERT INTO users (name,email,password,role) VALUES (?,?,?,?)",
        [name, email, hash, "driver"],
        (err) => {
            if (err) return res.json(err);
            res.json("Driver Added");
        }
    );
});

app.post('/create-trip', (req, res) => {
    const {
        driver_id,
        start,
        end,
        price,
        seats,
        startLat,
        startLng,
        endLat,
        endLng
    } = req.body;

    db.query(
        `INSERT INTO trips 
        (driver_id,start_location,end_location,price,available_seats,start_lat,start_lng,end_lat,end_lng) 
        VALUES (?,?,?,?,?,?,?,?,?)`,
        [driver_id, start, end, price, seats, startLat, startLng, endLat, endLng],
        (err, result) => {
            if (err) {
                console.log(err);
                return res.json({ error: err });
            }

            res.json({ tripId: result.insertId });
        }
    );
});
app.get('/trips', (req, res) => {
    db.query("SELECT * FROM trips", (err, result) => {
        if (err) {
            console.log(err);
            return res.json([]);
        }

        console.log("Trips:", result); // DEBUG
        res.json(result);
    });
});
app.post('/update-location', (req, res) => {
    const { driver_id, lat, lng } = req.body;

    db.query(
        "INSERT INTO locations (driver_id, latitude, longitude) VALUES (?,?,?)",
        [driver_id, lat, lng],
        (err) => {
            if (err) return res.json(err);
            res.json("Location Updated");
        }
    );
});
app.get('/driver-location/:driverId', (req, res) => {
    db.query(
        "SELECT * FROM locations WHERE driver_id=? ORDER BY updated_at DESC LIMIT 1",
        [req.params.driverId],
        (err, result) => {
            res.json(result[0]);
        }
    );
});
app.post('/notify', (req, res) => {
    const { message, type, bus_id } = req.body;

    db.query(
        "INSERT INTO notifications (message,type,bus_id) VALUES (?,?,?)",
        [message, type, bus_id],
        () => {

            // 🔥 SEND REAL-TIME
            io.emit("notification", {
                message,
                type,
                bus_id
            });

            res.json("Notification sent");
        }
    );
});

app.get('/notifications', (req, res) => {
    db.query("SELECT * FROM notifications", (err, result) => {
        res.json(result);
    });
});

app.post('/book-trip', (req, res) => {
    const { user_id, trip_id } = req.body;

    // 1. check available seats
    db.query("SELECT available_seats FROM trips WHERE id=?", [trip_id], (err, result) => {
        if (result[0].available_seats <= 0) {
            return res.json({ error: "No seats available" });
        }

        // 2. reduce seat
        db.query("UPDATE trips SET available_seats = available_seats - 1 WHERE id=?", [trip_id]);

        // 3. save booking
        db.query(
            "INSERT INTO bookings (user_id, trip_id) VALUES (?,?)",
            [user_id, trip_id],
            () => {
                res.json({ message: "Booking successful" });
            }
        );
    });
});



const server = http.createServer(app);

const io = new Server(server, {
    cors: { origin: "*" }
});

// ✅ NOW io is defined → safe to use
let buses = {};

io.on("connection", (socket) => {
    console.log("User connected");

    // 🚍 DRIVER SENDS LOCATION + DATA
    socket.on("sendLocation", (data) => {
        const d = Array.isArray(data) ? data[0] : data;

      const { 
    busId, lat, lng, 
    price, seats, countdown, 
    start, end,
    startLat, startLng, endLat, endLng
} = d;

if (!buses[busId]) {
    buses[busId] = {
        lat,
        lng,
        price,
        seats,
        countdown,
        start,
        end,
        startLat,
        startLng,
        endLat,
        endLng
    };
} else {
    Object.assign(buses[busId], {
        lat,
        lng,
        price,
        seats,
        countdown,
        start,
        end,
        startLat,
        startLng,
        endLat,
        endLng
    });
}

        io.emit("busUpdate", {
            busId,
            ...buses[busId]
        });
    });

    // ⏳ START COUNTDOWN
    socket.on("startCountdown", (data) => {
        const d = Array.isArray(data) ? data[0] : data;
        const { busId, time } = d;

        buses[busId].countdown = time;

        // 🔥 REAL-TIME COUNTDOWN LOOP
        const interval = setInterval(() => {
            if (buses[busId].countdown <= 0) {
                clearInterval(interval);
                return;
            }

            buses[busId].countdown--;

            io.emit("busUpdate", {
                busId,
                ...buses[busId]
            });

        }, 1000);
    });

    socket.on("disconnect", () => {
        console.log("User disconnected");
    });
});

// ✅ start server
server.listen(3000, () => {
    console.log("Server running on port 3000");
});