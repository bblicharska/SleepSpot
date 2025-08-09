
import { GroupListingDto, GroupMemberDto } from "../types/types";
import { fetchGroupMembers } from "../queries/fetchGroupMembers";


export const canUserDeleteListing = async (
  listing: GroupListingDto,
  userId: string | undefined,
  membersCache: { [groupId: string]: GroupMemberDto[] },
  updateCache: (groupId: string, members: GroupMemberDto[]) => void
): Promise<boolean> => {
  if (!userId) return false;

  try {
    let members: GroupMemberDto[];
    
    if (membersCache[listing.groupId]) {
      members = membersCache[listing.groupId];
    } else {
      members = await fetchGroupMembers(listing.groupId);
      updateCache(listing.groupId, members);
    }

    const userMembership = members.find((member) => member.userId === userId);
    return userMembership?.role === "Admin";
  } catch (error) {
    console.error("Error checking delete permission:", error);
    return false;
  }
};

export const checkDeletePermissionsForListings = async (
  listings: GroupListingDto[],
  userId: string | undefined,
  membersCache: { [groupId: string]: GroupMemberDto[] },
  updateCache: (groupId: string, members: GroupMemberDto[]) => void
): Promise<Set<string>> => {
  if (!userId) return new Set();

  const deletable = new Set<string>();

  for (const listing of listings) {
    const canDelete = await canUserDeleteListing(
      listing,
      userId,
      membersCache,
      updateCache
    );
    if (canDelete) {
      deletable.add(listing.id);
    }
  }

  return deletable;
};