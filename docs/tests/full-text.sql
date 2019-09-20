/*#Patterns: full-text */

SELECT Name, ListPrice
FROM Production.Product
WHERE ListPrice = 80.99
    /*#Error: full-text */
    AND CONTAINS(Name, 'Mountain')
GO
