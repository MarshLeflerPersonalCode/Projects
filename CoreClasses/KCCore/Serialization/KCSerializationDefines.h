//copyright Marsh Lefler 2000-...
#pragma once


//these defines help create the macro for defining what the get an set are for the serializer.
//the c# serialize app, builds these macro's by naming them KCSERIALIZEOBJECT_ plus line number.
//
//NOTE - not sure why I have to add the MACRO_HACK. It won't let me combine them without calling another macro function
#define KCCOMBINE_MACROS(MacroA,MacroB) MacroA##MacroB
#define KCCOMBINE_MACRO_HACK(MacroA,MacroB) KCCOMBINE_MACROS(MacroA,MacroB)
#define KCSERIALIZE_CODE(...) KCCOMBINE_MACRO_HACK(KCSERIALIZEOBJECT_, __LINE__);