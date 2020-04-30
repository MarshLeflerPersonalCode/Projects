#pragma once
#include "Utils/TypeDefinitions/KCPrimitiveTypesDefinition.h"

//copyright Marsh Lefler 2000-...
//Simple definition for defining seeds and macros for helping them be valid



//all seeds are int64s
typedef int64			KCSeed;

#define INVALID_SEED	-1

//returns if the seed is valid or not
#define isValidSeed(iSeed) (iSeed != INVALID_SEED && iSeed != 0)



