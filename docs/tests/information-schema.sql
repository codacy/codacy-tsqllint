/*#Patterns: information-schema */

/*#Error: information-schema*/
SELECT table_name FROM INFORMATION_SCHEMA.TABLES
    WHERE table_schema = 'MyTable'
