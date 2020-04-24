#pragma once
//copyright Marsh Lefler 2000-...
//bunch of macro's for helping debug
#include "KCDefines.h"

/////////////////////////////////////
//ASSERTS
//Look for macros at the end.
//
//KCEnsures will only do a debug break.  In release they won't do anything.
//
//KCCrit will exit with exit(1).
//
/////////////////////////////////////
#ifndef NDEBUG
#ifndef __linux__
#include <intrin.h>
#define KCASSERT_BREAK __nop();__debugbreak();
#else
#define KCASSERT_BREAK __debugbreak();
#endif
#endif

#ifdef USING_UE4

#else

#endif	//note using_ue4


#ifdef NDEBUG
#define _KCASSERT_(bAlwaysShow, mCondition, strMessage) 0
#else
#define _KCASSERT_(bAlwaysShow, mCondition, strMessage)\
{\
    static bool bAssertOccured = false;\
    if((bAlwaysShow == true || bAssertOccured == false) && !(mCondition))\
    {\
      bAssertOccured = true;\
      std::cerr << "Assertion failed: (" << #mCondition << "), "\
      << "function " << __FUNCTION__\
      << ", file " << __FILE__\
      << ", line " << __LINE__ << "."\
      << std::endl;\
      if(strMessage != "")\
      {\
          std::cerr << strMessage << std::endl;\
      }\
      KCASSERT_BREAK;\
    }\
}
#endif

#ifdef NDEBUG
#define _KCASSERT_RETURN_(bAlwaysShow, mCondition, strMessage)\
{\
    static bool bAssertOccured = false;\
    if((bAlwaysShow == true || bAssertOccured == false) && !(mCondition))\
    {\
      bAssertOccured = true;\
      return;\
    }\
}
#else
#define _KCASSERT_RETURN_(bAlwaysShow, mCondition, strMessage)\
{\
    static bool bAssertOccured = false;\
    if((bAlwaysShow == true || bAssertOccured == false) && !(mCondition))\
    {\
      bAssertOccured = true;\
      std::cerr << "Assertion failed: (" << #mCondition << "), "\
      << "function " << __FUNCTION__\
      << ", file " << __FILE__\
      << ", line " << __LINE__ << "."\
      << std::endl;\
      if(strMessage != "")\
      {\
          std::cerr << strMessage << std::endl;\
      }\
      KCASSERT_BREAK;\
      return;\
    }\
}
#endif
#ifdef NDEBUG
#define _KCASSERT_CONTINUE_(bAlwaysShow, mCondition, strMessage)\
{\
    static bool bAssertOccured = false;\
    if((bAlwaysShow == true || bAssertOccured == false) && !(mCondition))\
    {\
      bAssertOccured = true;\
      continue;\
    }\
}
#else
#define _KCASSERT_CONTINUE_(bAlwaysShow, mCondition, strMessage)\
{\
    static bool bAssertOccured = false;\
    if((bAlwaysShow == true || bAssertOccured == false) && !(mCondition))\
    {\
      bAssertOccured = true;\
      std::cerr << "Assertion failed: (" << #mCondition << "), "\
      << "function " << __FUNCTION__\
      << ", file " << __FILE__\
      << ", line " << __LINE__ << "."\
      << std::endl;\
      if(strMessage != "")\
      {\
          std::cerr << strMessage << std::endl;\
      }\
      KCASSERT_BREAK;\
      continue;\
    }\
}
#endif

#ifdef NDEBUG
#define _KCASSERT_RETURNVAL_(bAlwaysShow, mCondition, strMessage, mReturnValue)\
{\
    static bool bAssertOccured = false;\
    if((bAlwaysShow == true || bAssertOccured == false) && !(mCondition))\
    {\
      bAssertOccured = true;\
      return mReturnValue;\
    }\
}
#else
#define _KCASSERT_RETURNVAL_(bAlwaysShow, mCondition, strMessage, mReturnValue)\
{\
    static bool bAssertOccured = false;\
    if((bAlwaysShow == true || bAssertOccured == false) && !(mCondition))\
    {\
      bAssertOccured = true;\
      std::cerr << "Assertion failed: (" << #mCondition << "), "\
      << "function " << __FUNCTION__\
      << ", file " << __FILE__\
      << ", line " << __LINE__ << "."\
      << std::endl;\
      if(strMessage != "")\
      {\
          std::cerr << strMessage << std::endl;\
      }\
      KCASSERT_BREAK;\
      return mReturnValue;\
    }\
}
#endif


#ifdef NDEBUG
#define _KCCRIT_(mCondition, strMessage)  (!(condition)) ?exit(1):0
#else
#define _KCCRIT_(mCondition, strMessage)\
{\
    if(!(mCondition))\
    {\
      std::cerr << "Assertion failed: (" << #mCondition << "), "\
      << "function " << __FUNCTION__\
      << ", file " << __FILE__\
      << ", line " << __LINE__ << "."\
      << std::endl;\
      if(strMessage != "")\
      {\
          std::cerr << strMessage << std::endl;\
      }\
      KCASSERT_BREAK;\
      exit(1);\
    }\
}
#endif

///////////////////////ENSURES///////////////////////
#define KCEnsureAlways(condition )										_KCASSERT_(true, condition, "")
#define KCEnsureAlwaysReturn(condition )								_KCASSERT_RETURN_(true, condition, "");
#define KCEnsureAlwaysContinue(condition )								_KCASSERT_CONTINUE_(true, condition, "");
#define KCEnsureAlwaysReturnVal(condition, returnValue )				_KCASSERT_RETURNVAL_(true, condition, "", returnValue)
#define KCEnsureOnce(condition )										_KCASSERT_(false, condition, "")
#define KCEnsureOnceReturn(condition )									_KCASSERT_RETURN_(false, condition, "")
#define KCEnsureOnceContinue(condition )								_KCASSERT_CONTINUE_(false, condition, "")
#define KCEnsureOnceReturnVal(condition, returnValue )					_KCASSERT_RETURNVAL_(false, condition, "", returnValue)
#define KCEnsureAlwaysMsg(condition, message )							_KCASSERT_(true, condition, message)
#define KCEnsureAlwaysMsgReturn(condition, message )					_KCASSERT_RETURN_(true, condition, message)
#define KCEnsureAlwaysMsgContinue(condition, message )					_KCASSERT_CONTINUE_(true, condition, message)
#define KCEnsureAlwaysMsgReturnVal(condition, message, returnValue )	_KCASSERT_RETURNVAL_(true, condition, message, returnValue)
#define KCEnsuceOnceMsg(condition, message )							_KCASSERT_(false, condition, message)
#define KCEnsuceOnceMsgReturn(condition, message )						_KCASSERT_RETURN_(false, condition, message)
#define KCEnsuceOnceMsgContinue(condition, message )					_KCASSERT_CONTINUE_(false, condition, message)
#define KCEnsuceOnceMsgReturnVal(condition, message, returnValue )		_KCASSERT_RETURNVAL_(false, condition, message, returnValue)
///////////////////////CRIT///////////////////////
#define KCCrit(condition )												_KCCRIT_(condition, "")
#define KCCritMsg(condition, message )									_KCCRIT_(condition, message)