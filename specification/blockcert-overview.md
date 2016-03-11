## BlockCert: provable, open, verification of learner certifications from MOOC providers using the Bitcoin blockchain

### Abstract

Many MOOC **_(massive open online course)_** providers offer certifications on the completion of courses that they offer.  For learners, this represents a way for them to prove that they've taken and completed a course, or courses.  As employers increasingly find themselves receptive to considering experience coming from MOOCs, there is no decentralized-yet-authentic method for a third-party, like a prospective employer, to verify that a candidate has gotten the certifications they claim to.

BlockCert is an open specification that, with low-cost Bitcoin transactions, stores learner certifications on the Bitcoin blockchain -- a decentralized ledger with attractive security and integrity properties -- which can be introspected by anyone.  Utilizing the concept of Bitcoin addresses -- addresses are derived from private keys, so having the private key is tantamount to proving you "own" the address -- we can demonstrate that a MOOC provider, and other parties involved in a MOOC, have agreed that a learner has passed a course.  Third parties are then able to find these transactions on the blockchain and verify, for themselves, that the learner in fact passed the course.


### Key Concepts

All parties involved -- the MOOC provider, the organization who created the content, the course itself, and the learner who took the course -- have a unique Bitcoin address. This also means they must each hold their respective private key.

When a learner passes a course, a "multisignature" transaction is sent to the Bitcoin address of the learner from a special address which represents the MOOC provider, the organization, and the course.  Due to the nature of the Bitcoin protocol, this transaction is able to be proved, crytopgraphically, to be authentic and signed by the aforementioned parties.  This transaction contains a small amount of metadata that specifies an identifier value for each party.

When a third-party wishes to verify this transaction is legitimate, they can follow some simple steps; this would be handled automatically by any conforming verification provider running BlockCert-compatible software.

First, they would look at the transaction metadata and figure out the identifiers for the MOOC provider, organization, and course.  Providers and organizations are intended to use domain names as their identifier in order to take advantage of DNS (more on this later.)

Secondly, they would parse the Bitcoin transaction itself to come up with the individual addresses that formed the multisignature transaction.

Now with a list of identifiers and a list of Bitcoin addresses, they would check the DNS records for the provider and the organization.  Both of them would, if truly the owners of the given Bitcoin addresses, have a DNS record that listed the Bitcoin address.  This would be analogous to someone telling you:

"I received this item from a woman up the street who drives a red Ford Mustang with license plate 123ABC."

To verify that the received item came from who you were told it came from, you could go find the woman who owns the car, see if she has the keys to the car actually start it, and then ask if if she really gave that person the item.

--- TODO: explain course verification process

--- TODO: explain learner verification

--- TODO: explain Bitcoin network semantics, transaction fees, etc

--- TODO: explain metadata format, versioning, issuance/revocation, etc
