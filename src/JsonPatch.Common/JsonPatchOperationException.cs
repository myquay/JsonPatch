using System;

namespace JsonPatch
{
    /// <summary>
    /// Raised when an error occurs while processing changes. Includes extra details about what was being processed.
    /// </summary>
    public class JsonPatchOperationException : JsonPatchException
    {
        /// <summary>
        /// Json Patch Operation Type
        /// </summary>
        public JsonPatchOperationType operationType;

        /// <summary>
        /// Path of field being changed
        /// </summary>
        public string path;

        /// <summary>
        /// Underlying type of field
        /// </summary>
        public Type entityType;

        /// <summary>
        /// Value used for operation
        /// </summary>
        public object value;

        /// <summary>
        /// New instance of JsonPatchOperationException with message
        /// </summary>
        /// <param name="message"></param>
        public JsonPatchOperationException(string message) : base(message) { }

        /// <summary>
        /// New instance of JsonPatchOperationException with message and inner exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public JsonPatchOperationException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// New instance of JsonPatchOperationException with message and details of operation
        /// </summary>
        /// <param name="message"></param>
        /// <param name="operationType"></param>
        /// <param name="path"></param>
        /// <param name="entityType"></param>
        /// <param name="value"></param>
        public JsonPatchOperationException(string message, JsonPatchOperationType operationType, string path, Type entityType, object value) : this(message)
        {
            this.operationType = operationType;
            this.path = path;
            this.entityType = entityType;
            this.value = value;
        }
    }
}
