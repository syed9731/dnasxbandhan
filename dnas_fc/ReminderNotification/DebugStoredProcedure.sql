-- Debug script for GetNotRespondApprover stored procedure
-- Run this in SQL Server Management Studio or your preferred SQL client

-- 1. Get the stored procedure definition
PRINT '=== STORED PROCEDURE DEFINITION ==='
EXEC sp_helptext 'GetNotRespondApprover'

-- 2. Check if the stored procedure exists
PRINT '=== CHECKING IF STORED PROCEDURE EXISTS ==='
SELECT 
    SCHEMA_NAME(schema_id) AS SchemaName,
    name AS ProcedureName,
    create_date,
    modify_date
FROM sys.procedures 
WHERE name = 'GetNotRespondApprover'

-- 3. Check stored procedure parameters
PRINT '=== STORED PROCEDURE PARAMETERS ==='
SELECT 
    p.name AS ParameterName,
    t.name AS ParameterType,
    p.max_length,
    p.precision,
    p.scale,
    p.is_output,
    p.has_default_value,
    p.default_value
FROM sys.parameters p
INNER JOIN sys.types t ON p.user_type_id = t.user_type_id
WHERE p.object_id = OBJECT_ID('GetNotRespondApprover')
ORDER BY p.parameter_id

-- 4. Get the full stored procedure definition with line numbers
PRINT '=== STORED PROCEDURE WITH LINE NUMBERS ==='
SELECT 
    ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS LineNumber,
    definition
FROM sys.sql_modules 
WHERE object_id = OBJECT_ID('GetNotRespondApprover')

-- 5. Check for any dependencies or references
PRINT '=== DEPENDENCIES ==='
SELECT 
    OBJECT_NAME(referencing_id) AS ReferencingObject,
    OBJECT_DEFINITION(referencing_id) AS ReferencingDefinition
FROM sys.sql_expression_dependencies 
WHERE referenced_id = OBJECT_ID('GetNotRespondApprover')

-- 6. Test the stored procedure with error handling
PRINT '=== TESTING STORED PROCEDURE ==='
BEGIN TRY
    EXEC GetNotRespondApprover
    PRINT 'Stored procedure executed successfully'
END TRY
BEGIN CATCH
    PRINT 'Error executing stored procedure:'
    PRINT 'Error Number: ' + CAST(ERROR_NUMBER() AS VARCHAR(10))
    PRINT 'Error Message: ' + ERROR_MESSAGE()
    PRINT 'Error Line: ' + CAST(ERROR_LINE() AS VARCHAR(10))
    PRINT 'Error Procedure: ' + ISNULL(ERROR_PROCEDURE(), 'N/A')
END CATCH 