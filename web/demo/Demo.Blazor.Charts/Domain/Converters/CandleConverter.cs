﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Annium.Serialization.Json;
using NodaTime;

namespace Demo.Blazor.Charts.Domain.Converters;

public class CandleConverter : JsonConverter<Candle>
{
    public override Candle? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException($"Expected {JsonTokenType.StartArray}, got {reader.TokenType}");

        var currentDepth = reader.CurrentDepth;

        var timestamp = 0L;
        var open = 0m;
        var high = 0m;
        var low = 0m;
        var close = 0m;
        var index = 0;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray && reader.CurrentDepth == currentDepth)
            {
                if (timestamp == 0L)
                {
                    return null;
                }

                var candle = new Candle(Instant.FromUnixTimeMilliseconds(timestamp), open, high, low, close);

                return candle;
            }

            switch (index)
            {
                case 0:
                    timestamp = reader.GetInt64();
                    break;
                case 1:
                    open = reader.GetDecimalFromString();
                    break;
                case 2:
                    high = reader.GetDecimalFromString();
                    break;
                case 3:
                    low = reader.GetDecimalFromString();
                    break;
                case 4:
                    close = reader.GetDecimalFromString();
                    break;
                default:
                    reader.Skip();
                    break;
            }

            index++;
        }

        throw new JsonException("Unexpected end of json");
    }

    public override void Write(Utf8JsonWriter writer, Candle value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
