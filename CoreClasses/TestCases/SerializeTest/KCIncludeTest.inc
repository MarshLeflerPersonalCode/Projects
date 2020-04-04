#define KCSERIALIZEOBJECT_11 bool serialize(KCByteWriter &mByteWriter)\
{\
mByteWriter << m_strTest;\
return true;\
}\
\
bool serialize(KCDataGroup &mDataGroup, const KCString &strGroupName)\
{\
    if(mDataGroup.getGroupName().isEmpty()){ mDataGroup.setGroupName(strGroupName);}\
	mDataGroup.setProperty("m_strTest", m_strTest);\
	return true;\
}\
bool serialize(KCDataGroup &mDataGroup)\
{\
    return serialize(mDataGroup, "KCSerializeChild");\
}\
\
bool deserialize(KCByteReader &mByteReader)\
{\
	mByteReader << m_strTest;\
	return true;\
}\
\
bool deserialize(KCDataGroup &mDataGroup)\
{\
	m_strTest = mDataGroup.getProperty("m_strTest", m_strTest);\
	return true;\
}


#define KCSERIALIZEOBJECT_20 bool serialize(KCByteWriter &mByteWriter)\
{\
mByteWriter << m_fX;\
mByteWriter << m_fY;\
mByteWriter << m_fZ;\
m_SerializeChild.serialize(mByteWriter);\
return true;\
}\
\
bool serialize(KCDataGroup &mDataGroup, const KCString &strGroupName)\
{\
    if(mDataGroup.getGroupName().isEmpty()){ mDataGroup.setGroupName(strGroupName);}\
	mDataGroup.setProperty("m_fX", m_fX);\
	mDataGroup.setProperty("m_fY", m_fY);\
	mDataGroup.setProperty("m_fZ", m_fZ);\
	m_SerializeChild.serialize(mDataGroup, "m_SerializeChild");\
	return true;\
}\
bool serialize(KCDataGroup &mDataGroup)\
{\
    return serialize(mDataGroup, "KCIncludeTest");\
}\
\
bool deserialize(KCByteReader &mByteReader)\
{\
	mByteReader << m_fX;\
	mByteReader << m_fY;\
	mByteReader << m_fZ;\
	m_SerializeChild.deserialize(mByteReader);\
	return true;\
}\
\
bool deserialize(KCDataGroup &mDataGroup)\
{\
	m_fX = mDataGroup.getProperty("m_fX", m_fX);\
	m_fY = mDataGroup.getProperty("m_fY", m_fY);\
	m_fZ = mDataGroup.getProperty("m_fZ", m_fZ);\
	m_SerializeChild.deserialize(mDataGroup);\
	return true;\
}