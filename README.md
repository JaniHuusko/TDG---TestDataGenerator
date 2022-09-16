# Introduction 
Generates test data to sql database with connection string. 

# Getting Started
Currently you can insert data to one or all tables with the specified row count. works fine to at least 100k rows,
though some testing still required.
Reads schema specifics for column types and size, nullable- , computed- and identifier columns and relations with foreign keys.

Current accepted column types:
    bit
    varchar
    nvarchar
    tinyint
    smallint
    mediumint
    int
    bigint
    float
    decimal
    date
    datetime
    datetime2
    binary and varbinary only as nullable

Theres only ConsoleUI for now, but there is an idea for an UI where you could configure the values you want the app to insert.


# Ideas how to move forward
    1.  Create UI where you can configure the values e.g. datetime in future or past, any int as enum, 
        booleans that cant be true without conditions, insert string library to use..
    2.  Figure a way to insert all the possible scenarios with booleans in schema e.g. db has 10 different booleans so this
        creates test data that has all the possible combinations of these booleans, also maybe needs somekind of knowledge about
        how many rows of data is needed to fullfill these combinations.
    3.  Add more values that are possible to insert in SQL table.
    4.  Add other possible database schemas for example using mongoDB or cosmosDB.


# Contribute
If you have more ideas or just need some quick test data, please try it and give feedback. You can also just copy the repo 
and if you come up with an idea to improve you can create your own branch, implement changes and create a PR for main.