# BlockCert
A system for third-party authentication of course completions from MOOC providers.

# what is it?
BlockCert is a system, designed for MOOC providers, that uses the Bitcoin blockchain to permanently transact the completion of a MOOC.  By storing this information on the blockchain, we can provide an auditable history of a learner's accomplishments from a MOOC provider.

# how does it work?
A MOOC _provider_ offers _courses_ created by a particular _organization_.  For some MOOC providers, the organization might be the provider itself -- like Udacity -- and for others, these organizations might be actual universities -- like MIT offering courses on edx.org.

When a learner passes a course, it's an implicit agreement between the provider, the organization, and the course that the learner has passed.  We leverage this fact by creating a Bitcoin address for all four components -- provider, organization, course, and learner -- and sending a multisignature transaction from provider/organization/course to the learner.  We annotate this transaction with an OP_RETURN script to provide some metadata necessary to distinguish this transaction and later look up the finer points.

Now, we have a transaction that says a learner passed a course.  It's on the blockchain, durable and there for everyone to see.  How do we actually verify the multisignature transaction represents parties we care about, and that the learner address belongs to anyone claiming it? We send some more transactions!

For the _provider_ and _organization_, we send a transaction from their individual addresses to a valid-but-otherwise-arbitrary output address, which encodes metadata about how the authenticity of the address can be verified.  This could be a string pointing to a well-known domain under their control, which in turn has a DNS TXT record that lists out the given key.  It could also be a full URL (although shortened) to a page on their website, where users can clearly determine they're on a legitimate website, but otherwise wouldn't under fishing around through DNS record.

For the _learner_, we utilize a shared-knowledge approach.  One of the biggest reasons to verify that a learner has passed a course is to be able to show prospective employers what they've done.  Under normal employment, an employer will have access to private information about the learner.  Since this information can be expected to be handled sensitively, we can allow the learner to generate a message digest of certain bits of this information which is then used as metadata in a throwaway transaction, similar to the approach mentioned for providers/organizations.

This shared knowledge can be used by either side, so long as they use the same hash algorithm, to regenerate the message digest and thus prove they know the information.  By tying that information to a given address, which has received a transaction from a multisignature represented by other such verified addresses of providers/organizations/courses, we can form a chain of trust that extends from the MOOC provider all the way to the learner, and all publicly auditable, save for proving learner identity.
