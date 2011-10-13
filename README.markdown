DynamicPad is a play project to mimic linqpad, only for dynamic languages.
IronRuby is the language of DynamicPad for now, other languages might follow as needed
I use Massive from Rub Connery to include data access.

Given the following table structure:

	Firstname                                                                                            Birthdate  Lastname
	---------------------------------------------------------------------------------------------------- ---------- -----------
	Klaus                                                                                                2011-09-23 Hebsgaard 
	John                                                                                                 2011-09-01 Doe
	Jane                                                                                                 2000-09-01 Doe

You can do sql queries:

	result = tbl.Query("select * from person".ToString())
	outp = ""
	result.each do |bovs| 
		outp = outp + bovs.firstname
		outp = outp + bovs.lastname
		outp = outp + bovs.birthdate.ToString()
		outp = outp + "\n"
	end
	p outp
	
Result:

	Klaus                                                                                               Hebsgaard                                                                                           23-09-2011 00:00:00
	John                                                                                                Doe                                                                                                 01-09-2011 00:00:00
	Jane                                                                                                Doe                                                                                                 01-09-2000 00:00:00

You can also do templated queries, where you use objects to specify the query:
	
	x = tbl.Prototype

	x.firstname = "Klaus"

	DumpE tbl.Query("person".ToString(), x)
	
Result:

	Firstname: Klaus                                                                                               


	Birthdate: 23-09-2011 00:00:00


	Lastname: Hebsgaard       

If you wan't to do your own stuff, you can inherit Massives DynamicModel and do your own stuff, the connectionString is injected into the script as "connString":
	
	class MyModel < Massive::DynamicModel
		def GetStuff
			 Query("select * from Person Where firstname ='Klaus'".ToString())
		end	
	end

	m = MyModel.new(connString, "Person")
	#DumpE m.GetStuff
	#DumpE m.All
	p m.Count.ToString()

	s = m.Single("firstname = 'John'".ToString())
	Dump s

Result:

	4
	Firstname: John                                                                                                


	Birthdate: 01-09-2011 00:00:00


	Lastname: Doe     

	
RoadMap:
The plan is to add IronJs (maybe some coffeescript), IronPython and IronRuby support to create a kind of REPL/console application

Please note that this is a prototype - the architecture of this app is bad :-(

Found some inspiration here>
http://otac0n.com/blog/CommentView,guid,e07b25fa-c17a-4090-93cd-25fefa62b160.aspx
