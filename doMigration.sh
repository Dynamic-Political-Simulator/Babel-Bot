#!/bin/bash

# A script to automate the entire migration and database updating process.
# I have 0 experience in bash scripting so this might also just kill your linux machine lol
sudo docker ps | grep 'sql1' &> /dev/null
if [ $? != 0 ]; then
    echo "MSSQL docker instance not running!"
else
    echo "Deleting existing migrations..."
    cd ./BabelDatabase/Migrations
    rm -rf * # Delete all migrations
    cd ../..
    echo "Creating a new migration..."
    dotnet ef migrations add initial -c BabelContext -p ./BabelDatabase -s ./WOPR &> /dev/null
    echo "Updating the database..."
    yes "y" | dotnet ef database drop -c BabelContext -p ./BabelDatabase -s ./WOPR &> /dev/null
    dotnet ef database update -c BabelContext -p ./BabelDatabase -s ./WOPR &> /dev/null
    echo "Done."
fi