// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Elasticsearch.Analysis.TokenFilters;

public interface IPinYinTokenFilter : ITokenFilter
{
    /// <summary>
    /// when this option enabled, eg: 刘德华>ldh
    /// </summary>
    [DataMember(Name = "keep_first_letter")]
    bool KeepFirstLetter { get; set; }

    /// <summary>
    /// when this option enabled, will keep first letters separately
    /// eg: 刘德华>l,d,h, default: false
    /// NOTE: query result maybe too fuzziness due to term too frequency
    /// </summary>
    [DataMember(Name = "keep_separate_first_letter")]
    bool KeepSeparateFirstLetter { get; set; }

    /// <summary>
    ///  set max length of the first_letter result
    /// </summary>
    [DataMember(Name = "limit_first_letter_length")]
    int LimitFirstLetterLength { get; set; }

    /// <summary>
    /// when this option enabled, eg: 刘德华> [liu,de,hua]
    /// </summary>
    [DataMember(Name = "keep_full_pinyin")]
    bool KeepFullPinyin { get; set; }

    /// <summary>
    /// when this option enabled, eg: 刘德华> [liudehua]
    /// </summary>
    [DataMember(Name = "keep_joined_full_pinyin")]
    bool KeepJoinedFullPinyin { get; set; }

    /// <summary>
    /// keep non chinese letter or number in result
    /// </summary>
    [DataMember(Name = "keep_none_chinese")]
    bool KeepNoneChinese { get; set; }

    /// <summary>
    /// keep non chinese letter together
    /// eg: DJ音乐家 -> DJ,yin,yue,jia
    /// when set to false, eg: DJ音乐家 -> D,J,yin,yue,jia
    /// NOTE: keep_none_chinese should be enabled first
    /// </summary>
    [DataMember(Name = "keep_none_chinese_together")]
    bool KeepNoneChineseTogether { get; set; }

    /// <summary>
    /// keep non Chinese letters in first letter, eg: 刘德华AT2016->ldhat2016
    /// </summary>
    [DataMember(Name = "keep_none_chinese_in_first_letter")]
    bool KeepNoneChineseInFirstLetter { get; set; }

    /// <summary>
    /// keep non Chinese letters in joined full pinyin, eg: 刘德华2016->liudehua2016
    /// </summary>
    [DataMember(Name = "keep_none_chinese_in_joined_full_pinyin")]
    bool KeepNoneChineseInJoinedFullPinyin { get; set; }

    /// <summary>
    /// break non chinese letters into separate pinyin term if they are pinyin
    /// eg: liudehuaalibaba13zhuanghan -> liu,de,hua,a,li,ba,ba,13,zhuang,han
    /// NOTE: keep_none_chinese and keep_none_chinese_together should be enabled first
    /// </summary>
    [DataMember(Name = "none_chinese_pinyin_tokenize")]
    bool NoneChinesePinyinTokenize { get; set; }

    /// <summary>
    /// when this option enabled, will keep original input as well
    /// </summary>
    [DataMember(Name = "keep_original")]
    bool KeepOriginal { get; set; }

    /// <summary>
    /// lowercase non Chinese letters
    /// </summary>
    [DataMember(Name = "lowercase")]
    bool Lowercase { get; set; }

    [DataMember(Name = "trim_whitespace")]
    bool TrimWhitespace { get; set; }

    /// <summary>
    /// when this option enabled, duplicated term will be removed to save index, eg: de的>de
    /// NOTE: position related query maybe influenced
    /// </summary>
    [DataMember(Name = "remove_duplicated_term")]
    bool RemoveDuplicatedTerm { get; set; }

    /// <summary>
    /// after 6.0, offset is strictly constrained, overlapped tokens are not allowed
    /// with this parameter, overlapped token will allowed by ignore offset
    /// please note, all position related query or highlight will become incorrect
    /// you should use multi fields and specify different settings for different query purpose
    /// if you need offset, please set it to false
    /// </summary>
    [DataMember(Name = "ignore_pinyin_offset")]
    bool IgnorePinyinOffset { get; set; }
}
