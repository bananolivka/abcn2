using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using CN2.Core.DataStructures;

namespace CN2.Core.Algorithms
{
    public class CN2
    {
        private List<ProductionRule> _cover;
        private List<Tuple<ProductionRule, ArguedLearnableExample>> _arguedCover;

        /// <summary>
        /// Процент правильно распознанных примеров.
        /// </summary>
        // prce is percentage of correctly recognized examples
        private double _pcre;

        public List<ProductionRule> Cover
        {
            get { return _cover; }
        }

        public List<Tuple<ProductionRule, ArguedLearnableExample>> ArguedCover
        {
            get { return _arguedCover; }
        }

        /// <summary>
        /// Возвращает процент правильно распознанных примеров.
        /// </summary>
        public double PCRE
        {
            get { return _pcre; }
        }

        public CN2()
        {
            _cover = null;
            _arguedCover = null;
            _pcre = 0;
        }

        /// <summary>
        /// Производит обучение.
        /// </summary>
        /// <param name="learningExamples"></param>
        /// <param name="starSize"></param>
        /// <param name="heapSize"></param>
        /// <param name="isRandom"></param>
        public void Learn(List<LearnableExample> learningExamples, int starSize = 3, int heapSize = 3,
            bool isRandom = false)
        {
            if (learningExamples == null || learningExamples.Count == 0)
            {
                throw new ArgumentException("Недопустимый список атрибутов.");
            }
            if (starSize < 1)
            {
                throw new ArgumentException("Недопустимое значение StarSize: " + starSize + ".");
            }

            // список записей вида «значение типа решающего атрибута», «список положительных примеров», «список отрицательных примеров»
            List<Tuple<AttributeValue, List<LearnableExample>, List<LearnableExample>>> CArr =
                new List<Tuple<AttributeValue, List<LearnableExample>, List<LearnableExample>>>();

            // тип решающего атрибута
            AttributeType attributeType = learningExamples[0].DecisiveAttribute.Type;

            _cover = new List<ProductionRule>();

            // цикл по всем значениям типа решающего атрибута
            foreach (var value in attributeType.Values)
            {
                var cArr =
                    new Tuple<AttributeValue, List<LearnableExample>, List<LearnableExample>>(
                        new AttributeValue(attributeType, value),
                        new List<LearnableExample>(), new List<LearnableExample>());
                CArr.Add(cArr);

                // цикл по всем обучающим примерам
                foreach (var example in learningExamples)
                {
                    // если очередное значение типа решающего атрибута эквивалентно значению решающего атрибута очереднеого примера
                    if (value.Equals(example.DecisiveAttribute.Value))
                    {
                        cArr.Item2.Add(example);
                    }
                    else
                    {
                        cArr.Item3.Add(example);
                    }
                }

                Random random = new Random();
                int nextSeed = 0;

                // пока есть примеры в POS
                while (cArr.Item2.Count > 0)
                {
                    List<LearnableExample> NEG = new List<LearnableExample>(cArr.Item3);

                    nextSeed = isRandom ? random.Next(0, cArr.Item2.Count - 1) : nextSeed;
                    if (isRandom)
                    {
                        nextSeed = random.Next(0, cArr.Item2.Count - 1);
                    }

                    LearnableExample SEED = cArr.Item2[nextSeed];

                    if (!isRandom)
                    {
                        nextSeed++;
                    }

                    Expression STAR = new Expression(Operation.Con);

                    bool isStarCoversNEG = true;

                    int nextNeg = 0;

                    //todo обработать ситуацию бесконечного цикла
                    // пока условия STAR покрывают NEG (cArr.Item3)
                    while (isStarCoversNEG)
                    {
                        if (isRandom)
                        {
                            nextNeg = random.Next(0, NEG.Count - 1);
                        }

                        if (nextNeg >= NEG.Count)
                        {
                            nextSeed = 0;
                            break;
                        }

                        LearnableExample Eneg = NEG[nextNeg];

                        if (!isRandom)
                        {
                            nextNeg++;
                        }

                        Expression EXTENSION = new Expression(Operation.Con);

                        for (int i = 0; i < SEED.PredictiveAttributes.Count; i ++)
                        {
                            if (!SEED.PredictiveAttributes[i].Equals(Eneg.PredictiveAttributes[i]) &&
                                EXTENSION.Members.Count < heapSize)
                            {
                                EXTENSION.AddMember(new Expression(Operation.Dis,
                                    new List<IExpressionMember>()
                                    {
                                        SEED.PredictiveAttributes[i]
                                        /*, new Expression(Operation.Neg, Eneg.PredictiveAttributes[i])*/
                                    }));
                            }
                        }

                        if (!STAR.Members.Contains(EXTENSION))
                        {
                            STAR.AddMember(EXTENSION);
                        }
                        NEG.Remove(Eneg);

                        bool needBreak = true;
                        foreach (var example in NEG)
                        {
                            if (STAR.IsCover(example))
                            {
                                needBreak = false;
                                break;
                            }
                        }
                        if (needBreak)
                        {
                            break;
                        }
                    }

                    //todo удалить дублирующие устолия STAR

                    List<Tuple<IExpressionMember, int>> starExpressions = new List<Tuple<IExpressionMember, int>>();
                    foreach (var starExpression in STAR.Members)
                    {
                        int coveredExamplesCount = 0;
                        foreach (var example in cArr.Item2)
                        {
                            if (starExpression.IsCover(example))
                            {
                                coveredExamplesCount++;
                            }
                        }
                        starExpressions.Add(new Tuple<IExpressionMember, int>(starExpression, coveredExamplesCount));
                    }

                    // сортировка по возрастанию полезности
                    starExpressions = starExpressions.OrderBy(starExpression => starExpression.Item2).ToList();

                    if (STAR.Members.Count > starSize)
                    {
                        for (int i = 0; STAR.Members.Count > starSize; i ++)
                        {
                            STAR.Members.Remove(starExpressions[i].Item1);
                        }
                    }

                    IExpressionMember BEST = starExpressions.Last().Item1;
                    //COVER.AddMember(BEST);
                    _cover.Add(new ProductionRule(BEST, cArr.Item1));
                    for (int i = 0; i < cArr.Item2.Count; i ++)
                    {
                        if (BEST.IsCover(cArr.Item2[i]))
                        {
                            cArr.Item2.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Производит обучение по аргументированным примерам.
        /// </summary>
        /// <param name="learningExamples"></param>
        /// <param name="starSize"></param>
        /// <param name="heapSize"></param>
        /// <param name="isRandom"></param>
        public void Learn(List<ArguedLearnableExample> learningExamples, int starSize = 3, int heapSize = 3, bool isRandom = false)
        {
            Learn(new List<LearnableExample>(learningExamples), starSize, heapSize, isRandom);

            _arguedCover = new List<Tuple<ProductionRule, ArguedLearnableExample>>();

            foreach (var arguedLearnableExample in learningExamples)
            {
                foreach (var productionRule in _cover)
                {
                    if (_arguedCover.Select(rule => rule.Item1).ToList().Contains(productionRule))
                    {
                        continue;
                    }

                    if (productionRule.Condition.IsCover(arguedLearnableExample))
                    {
                        bool hasBecause = false;
                        foreach (var expressionMember in arguedLearnableExample.BecauseExpression.Members)
                        {
                            Expression productionRuleExpression = productionRule.Condition as Expression;
                            if (productionRuleExpression == null)
                            {
                                continue;
                            }

                            foreach (var member in productionRuleExpression.Members)
                            {
                                AttributeValue value = member as AttributeValue;
                                if (value == null)
                                {
                                    Expression expression = member as Expression;
                                    if (expression != null && expression.Members.Count == 1)
                                    {
                                        value = expression.Members.First() as AttributeValue;
                                        if (value == null)
                                        {
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }

                                if (value.Equals(expressionMember))
                                {
                                    hasBecause = true;
                                    break;
                                }
                            }
                            if (hasBecause)
                            {
                                break;
                            }
                        }
                        if (!hasBecause)
                        {
                            continue;
                        }

                        if (arguedLearnableExample.DespiteExpression != null)
                        {
                            bool hasDespite = false;
                            foreach (var expressionMember in arguedLearnableExample.DespiteExpression.Members)
                            {
                                Expression productionRuleExpression = productionRule.Condition as Expression;
                                if (productionRuleExpression == null)
                                {
                                    continue;
                                }

                                foreach (var member in productionRuleExpression.Members)
                                {
                                    AttributeValue value = member as AttributeValue;
                                    if (value == null)
                                    {
                                        Expression expression = member as Expression;
                                        if (expression != null & expression.Members.Count == 1)
                                        {
                                            value = expression.Members.First() as AttributeValue;
                                            if (value == null)
                                            {
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }

                                    if (value.Equals(expressionMember))
                                    {
                                        hasDespite = true;
                                        break;
                                    }
                                }
                                if (hasDespite)
                                {
                                    break;
                                }
                            }
                            if (hasDespite)
                            {
                                continue;
                            }
                        }

                        _arguedCover.Add(new Tuple<ProductionRule, ArguedLearnableExample>(productionRule,
                            arguedLearnableExample));
                    }
                }
            }
        }

        /// <summary>
        /// Производит экзамен.
        /// </summary>
        /// <param name="examineExamples"></param>
        /// <returns></returns>
        public List<Tuple<ExaminableExample, ProductionRule>> Examine(List<ExaminableExample> examineExamples)
        {
            // подготовка к экзамену

            // сипсок записей вида "экзаменуемый пример", "предсказываемое значение экзаменуемого примера"
            List<Tuple<ExaminableExample, List<ProductionRule>>> extendedExaminedExamples =
                new List<Tuple<ExaminableExample, List<ProductionRule>>>();
            foreach (var examineExample in examineExamples)
            {
                extendedExaminedExamples.Add(
                    new Tuple<ExaminableExample, List<ProductionRule>>(new ExaminableExample(examineExample),
                        new List<ProductionRule>()));
            }

            // список записей вида "продукционное правило из COVER", "количество примеров, покрываемых продукционным правилом из COVER"
            List<Tuple<ProductionRule, int>> extendedCover = new List<Tuple<ProductionRule, int>>();
            foreach (
                var productionRule in
                    (_arguedCover == null) ? _cover : _arguedCover.Select(arguedCover => arguedCover.Item1))
            {
                int coveredRulesCount = 0;
                foreach (var examineExample in examineExamples)
                {
                    if (productionRule.Condition.IsCover(examineExample))
                    {
                        coveredRulesCount ++;
                    }
                }
                extendedCover.Add(new Tuple<ProductionRule, int>(productionRule, coveredRulesCount));
            }

            // поиск значения по умолчанию

            List<Tuple<AttributeValue, int>> resultValuesRating = new List<Tuple<AttributeValue, int>>();
            foreach (var value in examineExamples[0].DecisiveAttribute.Type.Values)
            {
                int rating = 0;

                foreach (var examineExample in examineExamples)
                {
                    if (value == examineExample.DecisiveAttribute.Value)
                    {
                        rating++;
                    }
                }

                resultValuesRating.Add(
                    new Tuple<AttributeValue, int>(
                        new AttributeValue(examineExamples[0].DecisiveAttribute.Type, value), rating));
            }

            resultValuesRating = resultValuesRating.OrderByDescending(tuple => tuple.Item2).ToList();

            // определение правила по умолчанию

            List<AttributeValue> defaultAttributeTypes = new List<AttributeValue>();
            int defaultQ = 0;

            var decisiveAttributeType = examineExamples.First().DecisiveAttribute.Type;
            foreach (var value in decisiveAttributeType.Values)
            {
                int q = 0;
                foreach (var example in examineExamples)
                {
                    if (value.Equals(example.DecisiveAttribute.Value))
                    {
                        q++;
                    }
                }

                if (defaultAttributeTypes.Count == 0)
                {
                    defaultAttributeTypes.Add(new AttributeValue(decisiveAttributeType, value));
                    defaultQ = q;
                    continue;
                }

                if (q == defaultQ)
                {
                    defaultAttributeTypes.Add(new AttributeValue(decisiveAttributeType, value));
                    continue;
                }

                if (q > defaultQ)
                {
                    defaultAttributeTypes.Clear();
                    defaultAttributeTypes.Add(new AttributeValue(decisiveAttributeType, value));
                    defaultQ = q;
                    continue;
                }
            }

            // проведение экзамена

            foreach (var examinedExample in extendedExaminedExamples)
            {
                foreach (var extendedRule in extendedCover)
                {
                    if (extendedRule.Item1.Condition.IsCover(examinedExample.Item1))
                    {
                        examinedExample.Item2.Add(extendedRule.Item1);
                    }
                }

                // если ни одно продукционное правило из COVER не покрывает очередной экзаменуемый пример
                if (examinedExample.Item2.Count == 0)
                {
                    examinedExample.Item1.ExaminedAttribute = resultValuesRating[0].Item1;
                    examinedExample.Item2.Clear();
                    examinedExample.Item2.Add(new ProductionRule(defaultAttributeTypes.First(),
                        examinedExample.Item1.DecisiveAttribute) {IsDefault = true});

                    continue;
                }

                // если одно продукционное правило из COVER покрывает очередной экзаменуемый пример
                if (examinedExample.Item2.Count == 1)
                {
                    examinedExample.Item1.ExaminedAttribute = examinedExample.Item2[0].Result;
                    continue;
                }

                // если несколько продукционных правил из COVER покрывают очередной экзаменуемый пример
                if (examinedExample.Item2.Count > 1)
                {
                    Tuple<ProductionRule, int> bestProductionRule = extendedCover[0];
                    for (int i = 1; i < extendedCover.Count; i ++)
                    {
                        if (extendedCover[i].Item2 > bestProductionRule.Item2 &&
                            examinedExample.Item2.Contains(extendedCover[i].Item1))
                        {
                            bestProductionRule = extendedCover[i];
                        }
                    }
                    examinedExample.Item1.ExaminedAttribute = bestProductionRule.Item1.Result;
                    examinedExample.Item2.Clear();
                    examinedExample.Item2.Add(bestProductionRule.Item1);
                    continue;
                }
            }

            // ccre is count of correctly recognized examples
            double ccre = 0;

            List<Tuple<ExaminableExample, ProductionRule>> examinedExamples =
                new List<Tuple<ExaminableExample, ProductionRule>>();
            foreach (var extendedExaminedExample in extendedExaminedExamples)
            {
                examinedExamples.Add(new Tuple<ExaminableExample, ProductionRule>(extendedExaminedExample.Item1,
                    extendedExaminedExample.Item2.First()));
                if (
                    extendedExaminedExample.Item1.DecisiveAttribute.Value.Equals(
                        extendedExaminedExample.Item1.ExaminedAttribute.Value))
                {
                    ccre ++;
                }
            }

            _pcre = ccre/examinedExamples.Count;

            return examinedExamples;
        }

        ///// <summary>
        ///// Производит экзамен.
        ///// </summary>
        ///// <param name="examineExamples"></param>
        ///// <returns></returns>
        //public List<Tuple<ExaminableExample, ProductionRule, ArguedLearnableExample>> ArguedExamine(List<LearnableExample> examineExamples)
        //{
        //    // подготовка к экзамену

        //    // сипсок записей вида "экзаменуемый пример", "предсказываемое значение экзаменуемого примера"
        //    List<Tuple<ExaminableExample, List<ProductionRule>>> extendedExaminedExamples =
        //        new List<Tuple<ExaminableExample, List<ProductionRule>>>();
        //    foreach (var examineExample in examineExamples)
        //    {
        //        extendedExaminedExamples.Add(new Tuple<ExaminableExample, List<ProductionRule>>(new ExaminableExample(examineExample),
        //            new List<ProductionRule>()));
        //    }

        //    // список записей вида "продукционное правило из COVER", "количество примеров, покрываемых продукционным правилом из COVER"
        //    List<Tuple<ProductionRule, int, ArguedLearnableExample>> extendedCover = new List<Tuple<ProductionRule, int, ArguedLearnableExample>>();
        //    foreach (var productionRule in _arguedCover)
        //    {
        //        int coveredRulesCount = 0;
        //        foreach (var examineExample in examineExamples)
        //        {
        //            if (productionRule.Item1.Condition.IsCover(examineExample))
        //            {
        //                coveredRulesCount++;
        //            }
        //        }
        //        extendedCover.Add(new Tuple<ProductionRule, int, ArguedLearnableExample>(productionRule.Item1, coveredRulesCount, productionRule.Item2));
        //    }

        //    // поиск значения по умолчанию

        //    List<Tuple<AttributeValue, int>> resultValuesRating = new List<Tuple<AttributeValue, int>>();
        //    foreach (var value in examineExamples[0].DecisiveAttribute.Type.Values)
        //    {
        //        int rating = 0;

        //        foreach (var examineExample in examineExamples)
        //        {
        //            if (value == examineExample.DecisiveAttribute.Value)
        //            {
        //                rating++;
        //            }
        //        }

        //        resultValuesRating.Add(new Tuple<AttributeValue, int>(new AttributeValue(examineExamples[0].DecisiveAttribute.Type, value), rating));
        //    }

        //    resultValuesRating.OrderByDescending(tuple => tuple.Item2);

        //    // проведение экзамена

        //    foreach (var examinedExample in extendedExaminedExamples)
        //    {
        //        foreach (var extendedRule in extendedCover)
        //        {
        //            if (extendedRule.Item1.Condition.IsCover(examinedExample.Item1))
        //            {
        //                examinedExample.Item2.Add(extendedRule.Item1);
        //            }
        //        }

        //        // если ни одно продукционное правило из COVER не покрывает очередной экзаменуемый пример
        //        if (examinedExample.Item2.Count == 0)
        //        {
        //            //foreach (var resultValue in resultValuesRating)
        //            //{
        //            //	if (examinedExample.Item1.Result.Value.Equals(resultValue.Item1))
        //            //	{
        //            //		examinedExample.Item1.Value = re
        //            //	}
        //            //	break;
        //            //}

        //            examinedExample.Item1.ExaminedAttribute = resultValuesRating[0].Item1;

        //            continue;
        //        }

        //        // если одно продукционное правило из COVER покрывает очередной экзаменуемый пример
        //        if (examinedExample.Item2.Count == 1)
        //        {
        //            examinedExample.Item1.ExaminedAttribute = examinedExample.Item2[0].Result;
        //            continue;
        //        }

        //        // если несколько продукционных правил из COVER покрывают очередной экзаменуемый пример
        //        if (examinedExample.Item2.Count > 1)
        //        {
        //            Tuple<ProductionRule, int, ArguedLearnableExample> bestProductionRule = extendedCover[0];
        //            for (int i = 1; i < extendedCover.Count; i++)
        //            {
        //                if (extendedCover[i].Item2 > bestProductionRule.Item2 && examinedExample.Item2.Contains(extendedCover[i].Item1))
        //                {
        //                    bestProductionRule = extendedCover[i];
        //                }
        //            }
        //            examinedExample.Item1.ExaminedAttribute = bestProductionRule.Item1.Result;
        //            examinedExample.Item2.Clear();
        //            examinedExample.Item2.Add(bestProductionRule.Item1);
        //            continue;
        //        }
        //    }

        //    // ccre is count of correctly recognized examples
        //    double ccre = 0;

        //    List<Tuple<ExaminableExample, ProductionRule, ArguedLearnableExample>> examinedExamples =
        //        new List<Tuple<ExaminableExample, ProductionRule, ArguedLearnableExample>>();
        //    foreach (var extendedExaminedExample in extendedExaminedExamples)
        //    {
        //        examinedExamples.Add(new Tuple<ExaminableExample, ProductionRule, ArguedLearnableExample>(extendedExaminedExample.Item1, extendedExaminedExample.Item2.First(), extendedExaminedExample.Item1));
        //        if (
        //            extendedExaminedExample.Item1.DecisiveAttribute.Value.Equals(
        //                extendedExaminedExample.Item1.ExaminedAttribute.Value))
        //        {
        //            ccre++;
        //        }
        //    }

        //    _pcre = ccre / examinedExamples.Count;

        //    return examinedExamples;
        //}
    }
}
