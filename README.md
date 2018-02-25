# Structure Spider Advanced (Reverse Engineering)

#### Check [RELEASES](https://github.com/Stridemann/StructureSpiderAdvanced/releases) page for downloading latest stable version.

Scanning structures and substructures from initial address for values of defined types.

Supported applications:
- x64 bit
- x32 bit

Supported data types:
- Pointer
- Int (Int32)
- UInt (UInt32)
- Byte
- Long (Int64)
- Float
- String
- Unicode String


All settings have a description tooltip, so there is not sence to document all them here.
Using [Virtual Method Table](https://en.wikipedia.org/wiki/Virtual_method_table) search type is much more faster but can miss some pointers.
Program is targeted on PoE game. If you can't find your process in list of processes  - check "Process name filter".

Known bugs/problems:
- Restarting application between scan and filter items will cause error/crash of application (will be fixed soon)
- For x32 process can miss a lot of pointers if using "Memory Pages pointers checking", better to disable this option.
![Image](https://raw.githubusercontent.com/Stridemann/StructureSpiderAdvanced/master/Screenshot.png)
