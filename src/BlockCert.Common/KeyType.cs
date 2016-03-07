using System;

namespace BlockCert.Common
{
	/// <summary>
	/// Supported key types for BlockCert.
	/// </summary>
	public enum KeyType
	{
		/// <summary>
		/// A provider represents the service hosting a MOOC e.g. edX, Coursera, etc.
		/// </summary>
		Provider,
		/// <summary>
		/// An organization represents someone who has authored courses e.g. MIT, Udacity, etc.
		/// </summary>
		Organization,
		/// <summary>
		/// A course represents a given course as well as course run.  Harvard's CS50, a semester
		/// apart, would not represent the same "course" in the eyes of BlockCert.
		/// </summary>
		Course,
		/// <summary>
		/// A learner; someone who took a course, or courses.
		/// </summary>
		Learner
	}
}