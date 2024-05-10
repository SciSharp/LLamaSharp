# BatchedExecutor Fork - Generate Multiple Completions With Shared Memory

This example demonstrates using the `BatchedExecutor` to split one sequence into multiple sequences. See the source code [here](https://github.com/SciSharp/LLamaSharp/blob/master/LLama.Examples/Examples/BatchedExecutorFork.cs).

Sequences share memory up to the point they were split, meaning no extra memory is consumed by creating a fork. Inference runs for all sequences simultaneously, this means that running two sequences does _not_ take twice as much time as running one.

An example output, starting with the prompt `Not many people know that`:

```
Not many people know that
└── , in the 17th century, a military band led by Captain Charles
    ├──  Bossler of Baden, Germany, composed and played a music suite titled
    │   ├──  the "Civil Psalm," in order to rally German Protestants during
    │   │   ├──  the Thirty Years' War.  This tune became popular among German soldiers,
    │   │   │   ├──  and its popularity continued long after the war
    │   │   │   └──  and, eventually, reached France. The
    │   │   └──  the Thirty Years' War.This music, with its clear call
    │   │       ├──  to arms and strong Christian themes, helped
    │   │       └──  to arms and unwavering belief
    │   └──  "Baden's First National Symphony," with lyrics by a young Wol
    │       ├── fgang Amadeus Mozart. The story of the composition's creation
    │       │   ├──  has long been forgotten. But the B
    │       │   └──  was popularized by a novelty book
    │       └── fgang Amadeus Mozart. It's said that this music brought
    │           ├──  peace to Europe, at least for a
    │           └──  the troops together during difficult times. It
    └──  Newdick played a mournful dirge to accompany the procession of
        ├──  the head of King Charles I. It is the scene that opens my latest book
        │   ├── , "Death and Taxes." The book follows a British army captain named
        │   │   ├──  Marcus as he seeks revenge for his wife
        │   │   └──  William Darnay who becomes involved in
        │   └── , A King, A Pawn and a Prince. The murder of the king
        │       ├──  and the civil war that followed are the
        │       └──  is a watershed moment in the political
        └──  the coffin of William Shakespeare, as it was carried to its final resting place
            ├── . That is the least that can be said for a man who is often regarded
            │   ├──  as the greatest writer in the English language
            │   └──  as the greatest writer the English language has
            └──  at Stratford-upon-Avon.  Shakespeare, of course
                ├── , was a famous English poet and play
                └── , was one of the greatest playwright
```

Forked sequences can be used for many possible things. For example
 - Evaluating the system prompt once and forking for each independent conversation.
 - Saving a "checkpoint" in a conversation to return to later.
 - Beam Search.
 - Splitting a conversation, generating completions from several different "agents", and taking the best response.