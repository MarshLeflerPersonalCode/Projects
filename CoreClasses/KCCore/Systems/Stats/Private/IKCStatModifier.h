//copyright Marsh Lefler 2000-...
#pragma once
#include "KCStatInclude.h"

//////////////////////////////////////////////////////
//HOW TO USE
//
//This interface is for objects such as effects can modify stats.
//It gets used by the Stat Handler to figure out the final value of any one stat
//////////////////////////////////////////////////////




class IKCStatModifier
{
public:
	virtual ~IKCStatModifier()
	{
		_cleanStatModifier();
	};
	//returns the value of a stat fully calculated
	virtual bool						getValueCalculated(FKCStatCalculationParams &mParams, coreUnionData32Bit &mValue) { return getRawStatValue(mParams.m_iStatID, mValue); }
	//returns the stat value raw. Raw meaning that there hasn't been any calculations on the stat yet. Returning false means it doesn't support that stat
	virtual bool						getRawStatValue(KCStatID iStatID, coreUnionData32Bit &mValue) = 0;
	//Should return the unit type the stat owner is
	virtual KCUnitType					getUnitType() const = 0;
	//this gets called when a stat is relying on a stat that was just changed. For instance if a stat that relied on RANK to do a calculation and RANK was changed this function gets called with the stat relying on RANK
	virtual void						dirtyStat(KCStatID iStatID) = 0;
	//removes a child stat modifier
	virtual void						removeChildStatModifier(IKCStatModifier *pStatModifier)
	{
		KCEnsureAlwaysReturn(pStatModifier && pStatModifier->m_pStatModifierOwner);	//can't be null and the parent can't be null
		KCEnsureAlways(pStatModifier->m_pStatModifierOwner == this);	//this has to be the owner
		//this is a little weird but after getting asserts we should remove the child from it's owner - even if this isn't it's owner.
		pStatModifier->m_pStatModifierOwner->m_ChildStatModifiers.RemoveSwap(pStatModifier);	
		pStatModifier->setStatModifierOwner(nullptr);	//set the child owner to null
	}
	//adds a child stat modifier
	virtual void						addChildStatModifier(IKCStatModifier *pStatModifier)
	{
		KCEnsureAlwaysReturn(pStatModifier);	//can't be null
		m_ChildStatModifiers.AddUnique(pStatModifier);
		KCEnsureAlways(pStatModifier->m_pStatModifierOwner == nullptr);	//should be null!
		if (pStatModifier->m_pStatModifierOwner != nullptr)
		{
			pStatModifier->m_pStatModifierOwner->removeChildStatModifier(pStatModifier);
		}
		pStatModifier->setStatModifierOwner(this);
	}
	//returns the stat modifier owner
	FORCEINLINE IKCStatModifier *		getStatModifierOwner() { return m_pStatModifierOwner; }
	//returns the child stat modifiers
	FORCEINLINE KCTArray<IKCStatModifier * > &	getChildStatModifiers() { return m_ChildStatModifiers; }

protected:
	//cleans up the stat modifier - removing it from it's parent and removing it's children
	void								_cleanStatModifier()
	{
		if (getStatModifierOwner())
		{
			//scary - is this parent still valid?? hopefully it's a tight system. 
			getStatModifierOwner()->removeChildStatModifier(this);
		}
		while (m_ChildStatModifiers.Num())
		{
			removeChildStatModifier(m_ChildStatModifiers[0]);
		}
	}
	//sets the parent stat modifier
	virtual void						setStatModifierOwner(IKCStatModifier *pOwner) { m_pStatModifierOwner = pOwner; }
private:
	IKCStatModifier						*m_pStatModifierOwner = nullptr;
	KCTArray< IKCStatModifier * >		m_ChildStatModifiers;
};

