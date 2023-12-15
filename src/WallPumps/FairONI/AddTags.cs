using System;

namespace FairONI
{
    public static class AddTags
    {

        // Use TagManager.Create to create new tags, and then add strings here
        public static void AddStrings(Tag tag, String name)
        {
            Strings.Add(
                "STRINGS.MISC.TAGS." + tag.Name.ToUpperInvariant(),
                name
            );
        }

    }
}
