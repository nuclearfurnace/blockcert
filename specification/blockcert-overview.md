# BlockCert: provable, open, verification of learner certifications from MOOC providers using the Bitcoin blockchain

### Abstract

Many MOOC (_massive open online course_) providers offer certifications on the completion of courses that they offer.  For learners, this represents a way to prove that they've taken and completed a course, or courses.  As employers increasingly find themselves receptive to considering experience coming from MOOCs, there is no decentralized-yet-provable method for a third party, like a prospective employer, to verify that a candidate has received the certifications they claim to.

BlockCert is an open specification that, with low-cost Bitcoin transactions, stores a record of learner certifications on the Bitcoin blockchain -- a decentralized ledger with attractive security and integrity properties -- which can be inspected by anyone.  Utilizing the concept of Bitcoin addresses -- where addresses are derived from private keys, and thus having the private key is tantamount to proving you "own" the address -- we can demonstrate that a MOOC provider, and other parties involved in a MOOC, have agreed that a learner has passed a course.  Third parties are then able to find these transactions on the blockchain and verify, for themselves, that the learner has in fact passed the course.

### Key Concepts

All parties involved -- the MOOC provider, the organization who created the content, the course itself, and the learner who took the course -- have a unique Bitcoin address.  Bitcoin addresses are derived from a private key, so only the person with the private key for an address can ever send transactions from that address, or sign transactions on behalf of that address.

When a learner passes a course, a "multisignature" transaction is sent to the Bitcoin address of the learner.  A multisignature transaction is a transaction that must be signed by a configurable number of parties.  In our case, we sign it from the provider, the organization, and the course.  In conjunction with some extra metadata we embed, third parties can inspect these transactions, determine and verify the provider and organization that signed it, and ultimately verify what courses a learner passed.

For providers and organizations to be verified, we need a system that people trust which can be used to state "yes, this address belongs to us".  Luckily, we already have that system: DNS.  For example, people trust that `mit.edu` belongs to MIT.  If `mit.edu` has DNS records saying that Bitcoin address `abcdef` (not a real address) belongs to them, we would reasonably believe that it, in fact, belongs to them.  Thus, if a transaction states it is signed by provider A, and we look up the DNS information for provider A, and it points to one of the addresses used to sign the transaction, then we know that provider A is authentic and that their signature is valid.

--- TODO: explain course verification process

### Learner Verification

While we can store a record on the blockchain verifying that a learner has passed a course, there is still an issue of proving that you, the learner, are the owner of the address where the verification was sent.

Our current solution to this problem is to utilize "shared knowledge" -- pieces of information known only to you and to third parties who you cared to prove that you passed courses to.  When hashed cryptographically, these pieces of information can be stored in the blockchain, using the learner's address.  A third party can hash these values for themselves, and verify that they exists on the blockchain from the same address that received a certification.

In the case of employment, you usually share a few pieces of personal information with human resources: address, contact information, Social Security number, and more.  By and large, this information is treated as confidential and so there is a reasonable expectation that nobody else should know all of it.

For BlockCert, we leverage that fact by taking all of this information and generating a cryptographiclly-secure message digest of it.  This means that with the same information, anyone can generate the message digest and compare the output to see if it's the same.  It also means that it is computationally infeasible to reverse the message digest;  we can store it in plain sight.  Once we have the digest, we send a transaction from the learner's address with the digest in the metadata.  It's now stored, permanently and durably, on the blockchain.

For a third party to verify that a person is in fact the learner at a given address, they can perform the same message digest procedure based on information given from the person, and verify that the given address has sent a transaction with the same message digest.

Better still, as technology progresses and there are more ways to securely prove your identity -- standardized formats for fingerprint data, hardware tokens, etc -- further transactions can be sent from the same address, providing an upgrade path to use a continually stronger combination of pieces of information, making it harder and harder for someone to fake from the outside.

One thing to note is that this method of learner verification also leans on the human aspect: by utilizing personal information in the message digest, someone is less likely to take courses on behalf on another, and then provide this personal information to the other person for the purpose of being able to verify the courses elsewhere.  There are few pieces of information that both represent an individual uniquely and that are truly hard to prevent transfer of, or to fake.  Fingerprints and DNA are not easily spoofed by regular individuals, but there is a technical hurdle in being able to provide them to a generic third party in a consistent format and in a timely fashion.

While learner verification depends on strong pieces of information, we are still limited by current technical capabilities in this area.  Our protocol is designed with the intent to allow a natural upgrade path to better identifying marks if and when they discovered or created.

### Bitcoin Explained

The underlying technology that supports BlockCert is called Bitcoin.  Bitcoin is a "cryptocurrency": virtual currency that is transacted between computer systems, and where all transactions are cryptographically verified and stored on the "blockchain", a virtual ledger.

A ledger is a book where each page has space to record transactions -- sending or receiving money, and where who it's coming from and going to.  Not only does everyone involved with the Bitcoin blockchain have a copy of this ledger, they are able to say, with a very high degree of confidence, that the ledger is authentic: all the transactions within it are real.

As transactions are happening continuously, so is the generation of "blocks".  Among other things, transactions are recorded into these blocks.  Blocks hold a finite amount of transaction data, and also the signature of the block that came before it.  This signature represents the "fingerprint" of all the data in a block.

If an attacker wanted to change a transaction in a block, they would be changing the overall signature for that block.  Any blocks after it would also have their signatures change, as a cascading effect, since their signature depends on the signature of the block before them.

Since the blockchain is replicated across so many systems, and these systems operate in a majority mode -- if 3 systems think a change is valid, and 2 don't, the change happens -- it is not feasible for an attacker to change the "history" in the blockchain.  They would have to spend an extremely large sum of money to make sure enough systems accepted their modified version of history as valid.  Based on current network data, and the currently available mining systems, this cost would be in the tens of millions of dollars.

### BlockCert protocol

The BlockCert protocol is designed to occupy the 40 bytes of space available in an OP_RETURN script.  This means transaction data is kept small, often specifying pointers to other entities which can be further traversed in order to specify the full scope or context of an operation.

#### Freshness Warning!

The follow information on the BlockCert protocol should be considered a living reference: new versions of the protocol may be released in the future, potentially with backward-incompatible changes.  This document represents the first version (**Syncopate**, also referred to **BlockCert Syncopate**) of the protocol.

#### Transaction Header

All transactions start with three values: the mgic value, the version of the protocol, and the operation.  The magic value is two bytes, and the other values are each 1 byte.

The version of the protocol is not expected to change often, because there are only a few simple operations required to fulfill the BlockCert use case: issuing and revoking certifications, and the length of data available to us in OP_RETURN scripts is not likely to change.  It does give us the ability to support any changes to the Bitcoin protocol in the future, along with possibly migrating to other blockchains or systems entirely, such as Ethereum.

The operation is what the transaction is performing.  Currently, the only valid actions are to either issue (0x01) or revoke (0x02) a certification, or to sign (0x03) metadata about an address. {TODO: link this back to where we talk about how learner metadata is described}

#### Issuing/Revoking A Certificate

Issuance/revocation are the most common operations perform.  These occur when a learner passes a course, or potentially when a learner's certification is revoked due to extenuating circumstances.  Both operations, aside from the operation value in the transaction header, carry the exact same payload.

Both of these operations require the provider, the organization, and the course.  Provider and organization are compressed strings which should point to the primary domains used by both provider and organization, respectively.  Course is intended to be a variable (1 to 5 bytes) unsigned integer value which can be mapped back to the full course information.

Compression is used to shrink domain names down to the most tightly-packed representation possible.  This includes replacing common elements -- "www.", ".edu", ".com", etc -- with smaller values, let us fit an address like "mit.edu" into 4 bytes, or "www.cam.ac.uk" into 5 bytes.  This compression is achieved by coming up with, programmatically, a list of replacements -- i.e. "replace `www.` with `0x01`" -- and applying any possible replacement.  Since anyone trying to decode these transactions would need the same dictionary, these dictionaries will be tied to transaction versions, and specified fully for implementors to use.  This information can be found at the end of this document in the **Reference** section.

These transactions are sent from a multisig address, representative of the provider, organization and course addresses, to a learner address.  They are only valid when sent from a multisig address, because the multisig address is broken apart to find the public keys to verify against the DNS records for the given provider and organization.

##### Verification Flow

When a third-party wishes to verify an certification issuance, they follow the outlined steps here:

1. Find all multisig transactions where there is both an output going to the learner's address as well as an OP_RETURN script.

2. For each transaction found this way, check if the first two bytes of the OP_RETURN script match the BlockCert magic value.  This value is `0xAB 0xCD`.  If found, check if the third byte represents a valid BlockCert protocol version.

3. If the transaction so far looks valid, read the fourth byte to determine if this transaction is an issuance or revocation.  Implementors must scan all qualifying transactions to the learner's address to ensure they capture any revocations of a certification.

4. Read the provider and organization fields by consuming the remaining bytes until a NUL byte (`0x00`) is encountered, commonly known as a _C-Style_ string.  Repeat this twice to read both the provider string and the organization.

5. Decompress these strings using the version-specific dictionary for domain compression.  For every byte in the string, if the value is outside of the ASCII range (`<chr> & 0x80 == 0x80`), take that byte and look up its replacement.  Replace that single byte with the full replacement value.

6. Read the course identifier.  This is a 7-bit variable encoded unsigned integer.  Pseudocode for reading a 7-bit encoded value is given in the **Reference** section.

7. For both the provider and organization, retrieve all of the TXT records in DNS for the respective domains.

8. Decode the public keys that formed the multisig address of the transaction.

9. For both the provider and organization, look for a TXT record starting with `blockcert`.  If found, extract all data after the `:` character, and tokenize it on `,`.  Key/value pairs separated by `=` should remaining.

10. For each type -- provider and organization -- assert that the DNS TXT record has a key/value pair of `type=<value>` where `<value>` is equal to either `provider` or `organization` and that the record has a key/value pair of `address=<value>` where `<value>` is equal to one of the public keys in the multisig address. Take care to exclude public keys as they are matched up, so that both provider and organization cannot point to a single public key.

11. If both provider and organization have DNS TXT records that match to public keys in the multisig address, they are now verified and can be considered to be authentic.

12. The course information must be looked up from the provider using the course identifier.  The overall certification can be considered authentic at this point, but certification is effectively useless until the course information is retrieved, since that information will include the actual display name of the course.  This information is retrieved from the provider using the BlockCert provider interface API, which must be implemented by any provider that wants to participate.
