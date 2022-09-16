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
e.g. datetime in future or past, any int as enum, booleans that cant be true without conditions etc...

# Contribute
If you have more ideas or just need some quick test data, please try it and give feedback.