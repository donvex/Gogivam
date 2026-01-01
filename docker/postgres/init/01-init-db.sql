# PostgreSQL initialization script
-- Create the database if it doesn't exist
SELECT 'CREATE DATABASE deliveries_db' WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'deliveries_db')\gexec

-- Connect to the deliveries database
\c deliveries_db;

-- Create the deliverystatus enum type
DO $$ BEGIN
    CREATE TYPE deliverystatus AS ENUM ('Pending', 'Confirmed', 'InTransit', 'Delivered', 'Cancelled', 'Failed');
EXCEPTION
    WHEN duplicate_object THEN null;
END $$;

-- Create extensions if needed
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Grant privileges to the admin user
GRANT ALL PRIVILEGES ON DATABASE deliveries_db TO admin;
GRANT ALL ON SCHEMA public TO admin;