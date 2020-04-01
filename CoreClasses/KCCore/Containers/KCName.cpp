#include "KCName.h"
#include "KCIncludes.h"

static KCStringTable	g_StringTable;

KCStringTable & KCName::getStringTable()
{
	return g_StringTable;
}

KCName KCName::getNameFromID(uint32 iStringID)
{
	return g_StringTable.getStringByID(iStringID);
}

const KCString & KCName::toString() const
{
	return g_StringTable.getStringByID(m_iUniqueID);
}

uint32 KCName::_getIDFromString(const KCString &strString) const
{
	return g_StringTable.getStringID(strString);
}

void KCName::_setString(const KCString &strString)
{
	m_iUniqueID = g_StringTable.getStringID(strString);
#ifdef _DEBUG
	m_strString = g_StringTable.getStringByID(m_iUniqueID);
#endif
}
