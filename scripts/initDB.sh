#!/bin/bash
sqlite3 ../data/chirp.db < ../data/schema.sql;
sqlite3 ../data/chirp.db < ../data/dump.sql;

if [ $? -eq 0 ]; then
    echo "Database populated succesfully!";
else
    echo "Something went wrong with creating the database!";
fi