set term ^;
EXECUTE BLOCK
AS
    declare variable vsql varchar(5000);
begin

for
    SELECT 'ALTER TABLE '||r.rdb$relation_name ||
    ' DROP CONSTRAINT '||r.rdb$constraint_name||';'
    FROM rdb$relation_constraints r WHERE (r.rdb$constraint_type='FOREIGN KEY')
    INTO :VSQL
 do
    BEGIN
        EXECUTE STATEMENT :VSQL;

    END

end^

set term ;^

