﻿using System;
using System.Collections.Generic;
using System.Xml;
using Nuve.Condition;
using Nuve.Orthographic;
using Nuve.Orthographic.Action;

namespace Nuve.Reader
{
    internal static class OrthographyReader
    {
        private static Alphabet _alphabet;

        public static Orthography Read(XmlDocument xml)
        {
            _alphabet = ReadAlphabet(xml);
            IEnumerable<OrthographyRule> rules = ReadRules(xml);
            return new Orthography(_alphabet, rules);
        }

        private static Alphabet ReadAlphabet(XmlDocument xml)
        {
            XmlNode consonantsNode = xml.GetElementsByTagName("consonants")[0].FirstChild;
            XmlNode vowelsNode = xml.GetElementsByTagName("vowels")[0].FirstChild;
            string consonants = consonantsNode.Value;
            string vowels = vowelsNode.Value;
            return new Alphabet(consonants, vowels);
        }

        private static IEnumerable<OrthographyRule> ReadRules(XmlDocument xml)
        {
            XmlNodeList ruleNodeList = xml.GetElementsByTagName("rule");
            return RulesAsList(ruleNodeList);
        }


        private static IEnumerable<OrthographyRule> RulesAsList(XmlNodeList ruleNodeList)
        {
            var rules = new List<OrthographyRule>();
            foreach (XmlNode node in ruleNodeList)
            {
                rules.Add(ReadOrthographyRule(node));
            }
            return rules;
        }

        private static List<char> LettersAsList(XmlNodeList nodeList)
        {
            var list = new List<char>();
            foreach (XmlNode node in nodeList)
            {
                list.Add(node.InnerText.ToCharArray()[0]);
            }
            return list;
        }

        private static OrthographyRule ReadOrthographyRule(XmlNode ruleNode)
        {
            //string description = ruleNode["description"].InnerText;
            string id = ruleNode.Attributes["id"].InnerText;
            string level = ruleNode.Attributes["phase"].InnerText;
            List<Transformation> transforms = ReadTransformations(ruleNode);
            return new OrthographyRule(id, Int32.Parse(level), transforms);
        }

        private static List<Transformation> ReadTransformations(XmlNode ruleNode)
        {
            XmlNodeList transformNodeList = ruleNode.SelectNodes("transformation");
            var transforms = new List<Transformation>();
            foreach (XmlNode transformNode in transformNodeList)
            {
                transforms.Add(ReadTransform(transformNode));
            }
            return transforms;
        }

        private static Transformation ReadTransform(XmlNode transformNode)
        {
            string morpheme = transformNode.Attributes["morpheme"].InnerText;

            string actionName = transformNode.Attributes["action"].InnerText;

            string operandOne = transformNode.Attributes["operandOne"] != null
                ? transformNode.Attributes["operandOne"].InnerText
                : "";

            string operandTwo = transformNode.Attributes["operandTwo"] != null
                ? transformNode.Attributes["operandTwo"].InnerText
                : "";

            string flag = transformNode.Attributes["flag"] != null
                ? transformNode.Attributes["flag"].InnerText
                : "";

            BaseAction action = ActionFactory.Create(actionName, _alphabet, operandOne, operandTwo, flag);

            ConditionContainer conditions = ConditionContainer.EmptyContainer();

            if (transformNode.HasChildNodes)
            {
                conditions = ReadConditionContainer(transformNode.FirstChild);
            }

            return new Transformation(action, morpheme, conditions);
        }

        private static ConditionContainer ReadConditionContainer(XmlNode conditionsNode)
        {
            var conditions = new List<ConditionBase>();
            string flag = conditionsNode.Attributes["flag"].InnerText;

            if (conditionsNode.HasChildNodes)
            {
                conditions = ConditionsAsList(conditionsNode.ChildNodes);
            }

            return new ConditionContainer(conditions, flag);
        }

        private static List<ConditionBase> ConditionsAsList(XmlNodeList ruleNodeList)
        {
            var conditions = new List<ConditionBase>();
            foreach (XmlNode node in ruleNodeList)
            {
                conditions.Add(ReadCondition(node));
            }
            return conditions;
        }

        private static ConditionBase ReadCondition(XmlNode node)
        {
            string name = node.Attributes["operator"].InnerText;

            string morpheme = node.Attributes["morpheme"].InnerText;

            string operand = string.Empty;

            if (node.Attributes["operand"] != null)
            {
                operand = node.Attributes["operand"].InnerText;
            }

            return ConditionFactory.Create(name, morpheme, operand, _alphabet);
        }
    }
}