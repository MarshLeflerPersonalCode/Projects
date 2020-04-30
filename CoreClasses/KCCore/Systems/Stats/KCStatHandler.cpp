#include "KCStatHandler.h"
#include "Systems/Stats/KCStatsDefined.h"
#include "Systems/Stats/Private/KCStatDefinition.h"
#include "Systems/UnitTypes/KCDefinedUnitTypes.h"
#include "KCCoreData.h"

KCStatHandler::KCStatHandler(KCCoreData *pCoreData, KCUnitType eOwnerUnitType) :
	KCCoreObject(pCoreData)
{
	m_eUnitTypeOfOwner = eOwnerUnitType;
	if (UNITTYPES::IsA(eOwnerUnitType, UNITTYPES::ITEM))
	{
		m_pStats = KC_NEW KCStats(getCoreData(), ESTAT_HANDLER_TYPE::ITEM);
	}
	else
	{
		m_pStats = KC_NEW KCStats(getCoreData(), ESTAT_HANDLER_TYPE::CHARACTER);
	}
	
}

KCStatHandler::~KCStatHandler()
{
	_clean();
}



bool KCStatHandler::setRawValue(KCStatID iStatID, int32 iValue)
{
	KCEnsureAlwaysReturnVal(isValidStatID(iStatID), false);
	m_pStats->setRawValue(iStatID, iValue);
	_dirtyStat(iStatID);
	return true;
}

bool KCStatHandler::setRawValue(KCStatID iStatID, float fValue)
{
	KCEnsureAlwaysReturnVal(isValidStatID(iStatID), false);
	m_pStats->setRawValue(iStatID, fValue);
	_dirtyStat(iStatID);
	return true;
}

bool KCStatHandler::incrementRawValue(KCStatID iStatID, int32 iValueToAdd)
{
	KCEnsureAlwaysReturnVal(isValidStatID(iStatID), false);
	m_pStats->setRawValue(iStatID, m_pStats->getRawValue(iStatID, 0) + iValueToAdd);
	_dirtyStat(iStatID);
	return true;
}

bool KCStatHandler::incrementRawValue(KCStatID iStatID, float fValueToAdd)
{
	KCEnsureAlwaysReturnVal(isValidStatID(iStatID), false);
	m_pStats->setRawValue(iStatID, m_pStats->getRawValue(iStatID, 0.0f) + fValueToAdd);
	_dirtyStat(iStatID);
	return true;
}

FORCEINLINE coreUnionData32Bit _addValues(const FKCStatDefinition *pStatDefinition, coreUnionData32Bit mValue1, coreUnionData32Bit mValue2)
{
	switch (pStatDefinition->m_eStateAggregateType)
	{
	default:
	case ESTAT_AGGREGATE_TYPES::ADD:
		if (pStatDefinition->m_eStatType == ESTAT_PRIMITIVE_TYPES::INT32)
		{
			return mValue1.m_iValue32 + mValue2.m_iValue32;
		}
		else
		{
			return mValue1.m_fValue + mValue2.m_fValue;
		}

		break;
	case ESTAT_AGGREGATE_TYPES::SUBTRACT:
		if (pStatDefinition->m_eStatType == ESTAT_PRIMITIVE_TYPES::INT32)
		{
			return mValue1.m_iValue32 - mValue2.m_iValue32;
		}
		else
		{
			return mValue1.m_fValue - mValue2.m_fValue;
		}
		break;
	case ESTAT_AGGREGATE_TYPES::MULTIPLE:
		if (pStatDefinition->m_eStatType == ESTAT_PRIMITIVE_TYPES::INT32)
		{
			return mValue1.m_iValue32 * mValue2.m_iValue32;
		}
		else
		{
			return mValue1.m_fValue * mValue2.m_fValue;
		}
		break;
	}
}

FORCEINLINE bool _createParams(FKCStatCalculationParams &mParamsToFillOut, KCStatHandler *pStatHandler, KCStatID iStatID, ESTAT_INHERIT_TYPES eInheritType, KCUnitType eUnitTypeToIgnore)
{
	mParamsToFillOut.m_eInheritType = eInheritType;
	mParamsToFillOut.m_eUnitTypeToIgnore = eUnitTypeToIgnore;
	mParamsToFillOut.m_iStatID = iStatID;
	mParamsToFillOut.m_pOriginalParent = pStatHandler;
	mParamsToFillOut.m_pStatDefinition = pStatHandler->getStatDefinition(iStatID);
	mParamsToFillOut.m_pStatManager = pStatHandler->getStatManager();
	return mParamsToFillOut.isValid();
}

int32 KCStatHandler::getStatValueForWeapon(KCStatID iStatID, int32 iDefaultValue)
{
	KCEnsureAlwaysReturnVal(isValidStatID(iStatID), iDefaultValue);
	int32 iValue1 = getStatValue(iStatID, iDefaultValue, ESTAT_INHERIT_TYPES::SELF_AND_CHILDREN);
	int32 iValue2 = getStatValue(iStatID, iDefaultValue, ESTAT_INHERIT_TYPES::FULL_HIERARCHY, UNITTYPES::WEAPON);
	return _addValues(getStatDefinition(iStatID), iValue1, iValue2).m_iValue32;
}

float KCStatHandler::getStatValueForWeapon(KCStatID iStatID, float fDefaultValue)
{
	KCEnsureAlwaysReturnVal(isValidStatID(iStatID), fDefaultValue);
	float fValue1 = getStatValue(iStatID, fDefaultValue, ESTAT_INHERIT_TYPES::SELF_AND_CHILDREN);
	float fValue2 = getStatValue(iStatID, fDefaultValue, ESTAT_INHERIT_TYPES::FULL_HIERARCHY, UNITTYPES::WEAPON);
	return _addValues(getStatDefinition(iStatID), fValue1, fValue2).m_fValue;
}

int32 KCStatHandler::getStatValueSelfOnly(KCStatID iStatID, int32 iDefaultValue)
{
	return getStatValue(iStatID, iDefaultValue, ESTAT_INHERIT_TYPES::SELF, INVALID);
}

float KCStatHandler::getStatValueSelfOnly(KCStatID iStatID, float fDefaultValue)
{
	return getStatValue(iStatID, fDefaultValue, ESTAT_INHERIT_TYPES::SELF, INVALID);
}

int32 KCStatHandler::getStatValueFullHierarchy(KCStatID iStatID, int32 iDefaultValue, KCUnitType eUnitTypeToIgnore /*= INVALID*/)
{
	return getStatValue(iStatID, iDefaultValue, ESTAT_INHERIT_TYPES::FULL_HIERARCHY, eUnitTypeToIgnore);
}

float KCStatHandler::getStatValueFullHierarchy(KCStatID iStatID, float fDefaultValue, KCUnitType eUnitTypeToIgnore /*= INVALID*/)
{
	return getStatValue(iStatID, fDefaultValue, ESTAT_INHERIT_TYPES::FULL_HIERARCHY, eUnitTypeToIgnore);
}

int32 KCStatHandler::getStatValue(KCStatID iStatID, int32 iDefaultValue, ESTAT_INHERIT_TYPES eInheritType, KCUnitType eUnitTypeToIgnore)
{
	FKCStatCalculationParams mParams;
	coreUnionData32Bit mValue;
	coreUnionData32Bit mResult;
	KCEnsureAlwaysReturnVal(_createParams(mParams, this, iStatID, eInheritType, eUnitTypeToIgnore), iDefaultValue);
	KCEnsureAlwaysReturnVal(getValueCalculated(mParams, mValue), iDefaultValue);		
	if (_calculateGraphValue(mParams.m_pStatDefinition, mValue, mResult) == ESTAT_PRIMITIVE_TYPES::FLOAT)
	{
		KCEnsureOnce(false);	//asking for an int32 but the result is a float
		return (int32)mResult.m_fValue;
	}
	return mResult.m_iValue32;
}

float KCStatHandler::getStatValue(KCStatID iStatID, float fDefaultValue, ESTAT_INHERIT_TYPES eInheritType, KCUnitType eUnitTypeToIgnore)
{
	FKCStatCalculationParams mParams;
	coreUnionData32Bit mValue;
	coreUnionData32Bit mResult;
	KCEnsureAlwaysReturnVal(_createParams(mParams, this, iStatID, eInheritType, eUnitTypeToIgnore), fDefaultValue);
	KCEnsureAlwaysReturnVal(getValueCalculated(mParams, mValue), fDefaultValue);
	_calculateGraphValue(mParams.m_pStatDefinition, mValue, mResult);
	if (_calculateGraphValue(mParams.m_pStatDefinition, mValue, mResult) == ESTAT_PRIMITIVE_TYPES::INT32)
	{
		KCEnsureOnce(false);	//asking for an float but the result an int32
		return (float)mResult.m_iValue32;
	}
	return mResult.m_fValue;
}





bool KCStatHandler::getValueCalculated(FKCStatCalculationParams &mParams, coreUnionData32Bit &mValue)
{
	KCEnsureAlwaysReturnVal(mParams.isValid(), false);	
	std::unordered_map<int32, coreUnionData32Bit>::iterator mIter = m_CalculatedStats.find(mParams.getStatHashCode());
	if (mIter != m_CalculatedStats.end())
	{
		mValue = mIter->second;
		return true;
	}
	//is this even a valid stat for this handler?
	KCEnsureAlwaysReturnVal(m_pStats->isValidStatID(mParams.m_iStatID), false);

	/////////////////////////////////////////////////////////////////////
	//create entry for calculated stat
	m_CalculatedStats[mParams.getStatHashCode()] = coreUnionData32Bit();
	mIter = m_CalculatedStats.find(mParams.getStatHashCode());
	//we need to make sure we actually support this stat
	
	/////////////////////////////////////////////////////////////////////
	//Handle ESTAT_INHERIT_TYPES::FULL_HIERARCHY
	if (mParams.m_eInheritType == ESTAT_INHERIT_TYPES::FULL_HIERARCHY)
	{
		if (getStatModifierOwner() != null)
		{
			if (getStatModifierOwner()->getValueCalculated(mParams, mValue))
			{
				//TODO:
				//we need to do the graph lookup. Question is do we do the raw values all added up and then the graph or per stat modifier?
				mIter->second = mValue;
				return true;
			}
			return false;
		}
		//we are the parent. So now we only need children and ourselves.
		mParams.m_eInheritType = ESTAT_INHERIT_TYPES::SELF_AND_CHILDREN;
		mParams.resetHashCode();
	}

	//is this an even valid unit type!
	if (mParams.m_eUnitTypeToIgnore != INVALID &&
		UNITTYPES::IsA(m_eUnitTypeOfOwner, mParams.m_eUnitTypeToIgnore))
	{
		m_CalculatedStats.erase(mIter);
		return false;
	}

	/////////////////////////////////////////////////////////////////////
	//Get the value for the stat requested.
	if (mParams.m_pStatDefinition->m_eStatType == ESTAT_PRIMITIVE_TYPES::INT32)
	{
		mValue.m_iValue32 = m_pStats->getValue(mParams.m_iStatID, 0);
	}
	else
	{
		mValue.m_fValue = m_pStats->getValue(mParams.m_iStatID, 0.0f);
	}
	/////////////////////////////////////////////////////////////////////
	//Handle ESTAT_INHERIT_TYPES::SELF
	if (mParams.m_eInheritType == ESTAT_INHERIT_TYPES::SELF)
	{
		mIter->second = mValue;
		return true;
	}
	/////////////////////////////////////////////////////////////////////
	//Handle ESTAT_INHERIT_TYPES::CHILDREN		
	for (int32 iChildrenModifiers = 0; iChildrenModifiers < getChildStatModifiers().Num(); iChildrenModifiers++)
	{
		coreUnionData32Bit mChildValue(0);
		IKCStatModifier *pMondifer = getChildStatModifiers()[iChildrenModifiers];

		if ((mParams.m_eUnitTypeToIgnore != INVALID && UNITTYPES::IsA( pMondifer->getUnitType(), mParams.m_eUnitTypeToIgnore )) ||
			getChildStatModifiers()[iChildrenModifiers]->getValueCalculated(mParams, mChildValue) == false)
		{
			continue; //this child doesn't support this stat
		}
		mValue = _addValues(mParams.m_pStatDefinition, mValue, mChildValue);
	}
	/////////////////////////////////////////////////////////////
	//Done
	
	mIter->second = mValue;
	return true;
}

bool KCStatHandler::getRawStatValue(KCStatID iStatID, coreUnionData32Bit &mValue)
{
	KCEnsureAlwaysReturnVal(isValidStatID(iStatID), false);
	mValue = m_pStats->getRawValue(iStatID);
	return true;
}

void KCStatHandler::dirtyStat(KCStatID iStatID)
{
	KCEnsureAlwaysReturn(isValidStatID(iStatID));
	if( m_CalculatedStats.find(iStatID) == m_CalculatedStats.end() )	
	{
		return; //already dirty
	}
	
	_dirtyStat(iStatID);	
}

const FKCStatDefinition * KCStatHandler::getStatDefinition(KCStatID iStatID)
{
	KCEnsureAlwaysReturnVal(isValidStatID(iStatID), nullptr);
	const FKCStatDefinition *pStatDefinition = getStatManager()->getStatDefinitionByID(iStatID);
	KCEnsureAlwaysReturnVal(pStatDefinition, nullptr);
	return pStatDefinition;
}

void KCStatHandler::setStatModifierOwner(IKCStatModifier *pOwner)
{
	IKCStatModifier::setStatModifierOwner(pOwner);

}
coreUnionData32Bit KCStatHandler::getGraphValueForStatByValue(KCStatID iStatID, coreUnionData32Bit mValue)
{
	const FKCStatDefinition *pStatDefinition = getStatDefinition(iStatID);
	KCEnsureAlwaysReturnVal(pStatDefinition, mValue);
	coreUnionData32Bit mResult;
	_calculateGraphValue(pStatDefinition, mValue, mResult);
	return mResult;
}
ESTAT_PRIMITIVE_TYPES KCStatHandler::_calculateGraphValue(const FKCStatDefinition *pStatDefinition, coreUnionData32Bit mCurrentValue, coreUnionData32Bit &mResultValue)
{
	//NOTE this doesn't have any ensures because it all needs to be checked before hand.

	//TODO! Add graph manager in!

	if (pStatDefinition->m_strGraph.size() != 0)
	{
		if (pStatDefinition->m_eGraphResultStatType == ESTAT_PRIMITIVE_TYPES::INT32)
		{
			if (pStatDefinition->m_eStatType == ESTAT_PRIMITIVE_TYPES::INT32)
			{
				mResultValue.m_iValue32 = mCurrentValue.m_iValue32;
			}
			else
			{
				mResultValue.m_iValue32 = (int32)mCurrentValue.m_fValue;
			}
		}
		else  //else it's a float!
		{
			if (pStatDefinition->m_eStatType == ESTAT_PRIMITIVE_TYPES::INT32)
			{
				mResultValue.m_fValue = (float)mCurrentValue.m_iValue32;
			}
			else
			{
				mResultValue.m_fValue = mCurrentValue.m_fValue;
			}
		}
		return pStatDefinition->m_eGraphResultStatType;
	}
	mResultValue = mCurrentValue;
	return pStatDefinition->m_eStatType;
}

void KCStatHandler::_dirtyStat(KCStatID iStatID)
{
	const FKCStatDefinition *pStatDefinition = getStatDefinition(iStatID);
	KCEnsureAlwaysReturn(pStatDefinition);
	if (pStatDefinition->m_bDirtyAllStats)
	{		
		m_CalculatedStats.clear();
		if (getStatModifierOwner() != null)
		{
			getStatModifierOwner()->dirtyStat(iStatID);
		}
	}
	else
	{
		std::unordered_map<int32, coreUnionData32Bit>::iterator mIter = m_CalculatedStats.find(iStatID);
		if (mIter != m_CalculatedStats.end())
		{
			m_CalculatedStats.erase(mIter);
		}		
	}
	for (int32 iChildCount = 0; iChildCount < getChildStatModifiers().Num(); iChildCount++)
	{
		getChildStatModifiers()[iChildCount]->dirtyStat(iStatID);
	}
}

void KCStatHandler::_clean()
{
	DELETE_SAFELY(m_pStats);
	m_CalculatedStats.clear();
}
