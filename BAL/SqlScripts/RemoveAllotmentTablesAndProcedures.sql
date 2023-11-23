Declare @oName varchar(100),@oType varchar(10)
DECLARE  dupAllotObjects cursor
For  
	select name,[type] otype from sys.objects where type in ('U','P') and  (name like 'XT[_]%' or name like 'XP[_]%' or name like 'XApp[_]%') order by name
OPEN dupAllotObjects
Fetch dupAllotObjects into @oName,@oType
While @@fetch_Status=0
BEGIN
	if @oType='U'
		Exec('Drop table ' + @oName)
	else 
		Exec('Drop Proc ' + @oName)
	Fetch dupAllotObjects into @oName,@oType
END
Close dupAllotObjects
Deallocate dupAllotObjects

