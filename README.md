# Introduction 
Generates test data to sql database with connection string. 

# Use cases
1. Create test data for your project quickly and efficiently.
2. Test your projects performance with high amounts of data, or compare different solutions.
3. 

# Getting Started
Currently you can insert data to one or all tables with the specified row count. works fine to at least 100k rows with 6 tables,
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
    (all is non parametrized data)

Theres only ConsoleUI for now, but there is an idea for an UI where you could configure the values you want the app to insert.


# Ideas how to move forward
    1.  Create UI where you can configure the values e.g. datetime in future or past, any int as enum, 
        booleans that cant be true without conditions, insert string library to use..
    2.  Figure a way to insert all the possible scenarios with booleans in schema e.g. db has 10 different booleans so this
        creates test data that has all the possible combinations of these booleans, also maybe needs somekind of knowledge about
        how many rows of data is needed to fullfill these combinations.
    3.  Add more values that are possible to insert in SQL table.
    4.  Add other possible database schemas for example using mongoDB or cosmosDB.
    5.  Currently I've been able to add 100k rows easily under a minute and 1 million rows to a row without relations, but 
        Visua studio crashes if trying to add 1 million rows to a row with relations, so if there is some optimization ideas
        on how to add 1 million rows to relations and even for all rows, would be cool but im pretty satisfied with 100k rows.
    6.  Create UI property where you can first read the test data that was generated and pick the data you want to remove 
        from the data and an AI that can calculate which kind of data user wants to remove and makes suggestions for it.


# Contribute
If you have more ideas or just need some quick test data, please try it and give feedback. You can also just copy the repo 
and if you come up with an idea to improve you can create your own branch, implement changes and create a PR for main.