# RandomGenerator Class

Provides methods for generating cryptographically strong random data.

    [Obsolete]
    public abstract class RandomGenerator

!!! Note
    This type is obsolete and will be removed in a future version. The recommended alternative is `System.Security.Cryptography.RandomNumberGenerator`.


## [TOC] Summary


## Static Properties


### Default

Gets the default random generator.

    public static RandomGenerator Default { get; }

#### Remarks

The default random generator returns cryptographically strong random data
generated by the operating system:

* On Windows systems, the `RtlGenRandom` function is used.
* On recent Linux kernels, the `getrandom` system call is used.
* On other Unices, the `/dev/urandom` device is used.


## Methods


### GenerateBytes(int)

Generates a cryptographically strong random sequence of values and returns it as
an array of bytes.

    public byte[] GenerateBytes(
        int count)

#### Parameters

count
: The number of bytes to generate.

#### Return Value

An array of bytes that contains the generated values.

#### Exceptions

ArgumentOutOfRangeException
: `count` is less than 0.


### GenerateBytes(Span<byte>)

Fills a span of bytes with a cryptographically strong random sequence of values.

    public void GenerateBytes(
        Span<byte> bytes)

#### Parameters

bytes
: The span to fill with random values.


### GenerateInt32()

Returns a non-negative random integer.

    public int GenerateInt32()

#### Return Value

A 32-bit signed integer that is greater than or equal to 0 and less than or
equal to `int.MaxValue`.


### GenerateInt32(int)

Returns a non-negative random integer that is less than the specified maximum.

    public int GenerateInt32(
        int upperExclusive)

#### Parameters

upperExclusive
: The exclusive upper bound of the random number to be generated. `upperExclusive`
    must be greater than or equal to 0.

#### Return Value

A 32-bit signed integer that is greater than or equal to 0 and less than
`upperExclusive`. However, if `upperExclusive` equals 0, `upperExclusive` is returned.

#### Exceptions

ArgumentOutOfRangeException
: `upperExclusive` is less than 0.


### GenerateInt32(int, int)

Returns a random integer that is within a specified range.

    public int GenerateInt32(
        int lowerInclusive,
        int upperExclusive)

#### Parameters

lowerInclusive
: The inclusive lower bound of the random number to be generated.

upperExclusive
: The exclusive upper bound of the random number to be generated. `upperExclusive`
    must be greater than or equal to `lowerInclusive`.

#### Return Value

A 32-bit signed integer greater than or equal to `lowerInclusive` and less than
`upperExclusive`. However, if `lowerInclusive` equals `upperExclusive`, `lowerInclusive` is returned.

#### Exceptions

ArgumentException
: `lowerInclusive` is greater than `upperExclusive`.


### GenerateKey(Algorithm, in KeyCreationParameters)

Generates a new cryptographic key for the specified algorithm.

    public Key GenerateKey(
        Algorithm algorithm,
        in KeyCreationParameters creationParameters = default)

#### Parameters

algorithm
: The algorithm for the key.

creationParameters
: A [[KeyCreationParameters|KeyCreationParameters Struct]] value that specifies
    advanced parameters for the creation of the [[Key|Key Class]] instance.

#### Return Value

A new instance of the [[Key|Key Class]] class that represents the new key.

#### Exceptions

ArgumentNullException
: `algorithm` is `null`.

NotSupportedException
: The specified algorithm does not use keys.


### GenerateUInt32()

Returns a random integer.

    public uint GenerateUInt32()

#### Return Value

A 32-bit unsigned integer that is greater than or equal to 0 and less than or
equal to `uint.MaxValue`.


### GenerateUInt32(uint)

Returns a random integer that is less than the specified maximum.

    public uint GenerateUInt32(
        uint upperExclusive)

#### Parameters

upperExclusive
: The exclusive upper bound of the random number to be generated.

#### Return Value

A 32-bit unsigned integer that is greater than or equal to 0 and less than
`upperExclusive`. However, if `upperExclusive` equals 0, `upperExclusive` is returned.


### GenerateUInt32(uint, uint)

Returns a random integer that is within a specified range.

    public uint GenerateUInt32(
        uint lowerInclusive,
        uint upperExclusive)

#### Parameters

lowerInclusive
: The inclusive lower bound of the random number to be generated.

upperExclusive
: The exclusive upper bound of the random number to be generated. `upperExclusive`
    must be greater than or equal to `lowerInclusive`.

#### Return Value

A 32-bit unsigned integer greater than or equal to `lowerInclusive` and less than
`upperExclusive`. However, if `lowerInclusive` equals `upperExclusive`, `lowerInclusive` is returned.

#### Exceptions

ArgumentException
: `lowerInclusive` is greater than `upperExclusive`.


## Thread Safety

All members of this type are thread safe.
