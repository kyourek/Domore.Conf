﻿using System;
using System.ComponentModel;

namespace Domore.Configuration.Helpers {
    class Converter {
        public object Convert(Type type, string value) {
            return TypeDescriptor.GetConverter(type).ConvertFrom(value);
        }

        public object Convert(Type type, IConfigurationBlock block, string key) {
            if (null == type) throw new ArgumentNullException(nameof(type));
            if (null == block) throw new ArgumentNullException(nameof(block));

            var conv = TypeDescriptor.GetConverter(type);
            if (conv is ConfigurationTypeConverter conf) {
                conf.ConfigurationBlock = block;
            }

            if (block.ItemExists(key)) {
                var value = block.Item(key).OriginalValue;
                try {
                    return conv.ConvertFrom(value);
                }
                catch {
                    if (type == typeof(Type)) {
                        return Type.GetType(value, throwOnError: true, ignoreCase: true);
                    }
                    var valueType = Type.GetType(value, throwOnError: false, ignoreCase: true);
                    if (valueType != null) {
                        return Activator.CreateInstance(valueType);
                    }
                    throw;
                }
            }

            try {
                return conv.ConvertFrom(key);
            }
            catch {
                var constructor = type.GetConstructor(new Type[] { });
                if (constructor != null) {
                    return constructor.Invoke(new object[] { });
                }
                throw;
            }
        }
    }
}
