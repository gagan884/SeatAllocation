/******************Backup and truncate table****************
select name from Sys.tables where name like 'R[0-9][_]%' order by name
select * from sys.tables order by create_date desc
select * from Sys.tables where name not like 'R[0-9][_]%' order by name
select 'select * into dbo.R2_'+name+' from '+name from Sys.tables where name not like 'R[0-9][_]%' order by name
select 'truncate table '+name from Sys.tables where name not like 'R[0-9][_]%' order by name
select 'select '''+name+''', count(*) from '+name from Sys.tables where name not like 'R[0-9][_]%' order by name
select 'Exec sp_rename ''dbo.'+name+ ''',''R1'+substring(name,3, len(name))+'''' from sys.tables where name like 'R2%' order by name
--*************************************************************/

/******************************Add Account**********************/
select * from XT_Administrator
-------------------------------------************************/


select replace(substring('03.G1.WBJEE.HS.BA.NO.B',13,8),'.','') from XT_AllotmentSummary

select '1initseats' Cat,AIOPNO,HSOPNO,HSOPPH,HSBANO,HSBAPH,HSBBNO,HSBBPH,HSSCNO,HSSCPH,HSSTNO,HSSTPH,HSNOTF
from 
(
select replace(substring(sequence,13,8),'.','') catsubcat,sum(initseats) total  from XT_AllotmentSummary group by replace(substring(sequence,13,8),'.','')
) A
pivot
(
sum(total)
for catsubcat in (AIOPNO,HSOPNO,HSOPPH,HSBANO,HSBAPH,HSBBNO,HSBBPH,HSSCNO,HSSCPH,HSSTNO,HSSTPH,HSNOTF)
) B
union
select '2newtseats' Cat,AIOPNO,HSOPNO,HSOPPH,HSBANO,HSBAPH,HSBBNO,HSBBPH,HSSCNO,HSSCPH,HSSTNO,HSSTPH,HSNOTF
from 
(
select replace(substring(sequence,13,8),'.','') catsubcat,sum(NewSeats) total  from XT_AllotmentSummary group by replace(substring(sequence,13,8),'.','')
) A
pivot
(
sum(total)
for catsubcat in (AIOPNO,HSOPNO,HSOPPH,HSBANO,HSBAPH,HSBBNO,HSBBPH,HSSCNO,HSSCPH,HSSTNO,HSSTPH,HSNOTF)
) B
union
select '3allottedseats' Cat,AIOPNO,HSOPNO,HSOPPH,HSBANO,HSBAPH,HSBBNO,HSBBPH,HSSCNO,HSSCPH,HSSTNO,HSSTPH,HSNOTF
from 
(
select replace(substring(sequence,13,8),'.','') catsubcat,sum(allotted) total  from XT_AllotmentSummary group by replace(substring(sequence,13,8),'.','')
) A
pivot
(
sum(total)
for catsubcat in (AIOPNO,HSOPNO,HSOPPH,HSBANO,HSBAPH,HSBBNO,HSBBPH,HSSCNO,HSSCPH,HSSTNO,HSSTPH,HSNOTF)
) B
union 
select '4vacancy' Cat,AIOPNO,HSOPNO,HSOPPH,HSBANO,HSBAPH,HSBBNO,HSBBPH,HSSCNO,HSSCPH,HSSTNO,HSSTPH,HSNOTF
from 
(
select replace(substring(sequence,13,8),'.','') catsubcat,sum(NewSeats-Allotted) total  from XT_AllotmentSummary group by replace(substring(sequence,13,8),'.','')
) A
pivot
(
sum(total)
for catsubcat in (AIOPNO,HSOPNO,HSOPPH,HSBANO,HSBAPH,HSBBNO,HSBBPH,HSSCNO,HSSCPH,HSSTNO,HSSTPH,HSNOTF)
) B
